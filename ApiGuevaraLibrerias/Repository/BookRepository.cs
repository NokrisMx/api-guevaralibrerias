using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _db;

    public BookRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> BookExists(int id)
    {
        return await _db.Books.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> BookExistsByISBN(string isbn, int excludeId = 0)
    {
        return await _db.Books.AnyAsync(b =>
            b.ISBN == isbn && b.Id != excludeId);
    }

    public async Task<Book> CreateBook(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        await _db.Books.AddAsync(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<bool> DeleteBook(int id)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
            return false;

        // Eliminar imagen del servidor si existe
        if (!string.IsNullOrEmpty(book.ImgUrlLocal) && File.Exists(book.ImgUrlLocal))
            File.Delete(book.ImgUrlLocal);

        _db.Books.Remove(book);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Book>> GetBooks()
    {
        return await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<Book?> GetBook(int id)
    {
        return await _db.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> UpdateBook(Book book)
    {
        var existing = await _db.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (existing == null)
            return null;

        // Si hay nueva imagen, eliminar la anterior
        if (!string.IsNullOrEmpty(book.ImgUrlLocal) &&
            !string.IsNullOrEmpty(existing.ImgUrlLocal) &&
            existing.ImgUrlLocal != book.ImgUrlLocal &&
            File.Exists(existing.ImgUrlLocal))
        {
            File.Delete(existing.ImgUrlLocal);
        }

        existing.Title = book.Title;
        existing.Description = book.Description;
        existing.Price = book.Price;
        existing.ISBN = book.ISBN;
        existing.Stock = book.Stock;
        existing.CategoryId = book.CategoryId;
        existing.AuthorId = book.AuthorId;
        existing.ImgUrl = book.ImgUrl;
        existing.ImgUrlLocal = book.ImgUrlLocal;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> UpdateStock(int bookId, int newStock)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
        if (book == null)
            return false;

        book.Stock = newStock;
        book.UpdatedAt = DateTime.UtcNow;
        return await _db.SaveChangesAsync() > 0;
    }

}