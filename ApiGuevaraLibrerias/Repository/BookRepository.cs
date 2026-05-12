using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Mapster;
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

    public async Task<ApiResponse<BookDto>> GetBook(int id)
    {
        var book = await _db.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher).FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return new ApiResponse<BookDto>
            {
                Success = false,
                Message = "Libro no encontrado.",
                Data = null
            };
        }
        return new ApiResponse<BookDto>
        {
            Success = true,
            Message = "Libro obtenido correctamente.",
            Data = book.Adapt<BookDto>()
        };
    }

    public async Task<ApiResponse<IEnumerable<BookDto>>> GetBooks()
    {
        var books = await _db.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher).ToListAsync();
        if (books == null)
        {
            return new ApiResponse<IEnumerable<BookDto>>
            {
                Success = false,
                Message = "Libros no encontrados",
                Data = null
            };
        }
        return new ApiResponse<IEnumerable<BookDto>>
        {
            Success = true,
            Message = "Libros obtenidos correctamente.",
            Data = books.Adapt<IEnumerable<BookDto>>()
        };
    }

    public IQueryable<Book> GetBooksQuery()
    {
        return _db.Books
            .Include(b => b.Category)
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .AsNoTracking();
    }

    public async Task<ApiResponse<BookDto>> CreateBook(Book book)
    {
        if (await BookExistsByISBN(book.ISBN))
        {
            return new ApiResponse<BookDto>
            {
                Success = false,
                Message = "Ya existe un libro con ese ISBN",
                Data = null
            };
        }

        book.CreatedAt = DateTime.UtcNow;

        await _db.Books.AddAsync(book);
        await _db.SaveChangesAsync();

        var createdBook = await _db.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher).FirstOrDefaultAsync(b => b.Id == book.Id);

        return new ApiResponse<BookDto>
        {
            Success = true,
            Message = "Libro creado correctamente",
            Data = createdBook!.Adapt<BookDto>()
        };
    }

    public async Task<ApiResponse<BookDto>> UpdateBook(Book book)
    {
        var existing = await _db.Books.FirstOrDefaultAsync(b => b.Id == book.Id);

        if (existing == null)
        {
            return new ApiResponse<BookDto>
            {
                Success = false,
                Message = "Libro no encontrado.",
                Data = null
            };
        }

        if (await BookExistsByISBN(book.ISBN, book.Id))
        {
            return new ApiResponse<BookDto>
            {
                Success = false,
                Message = "Ya existe un libro con ese ISBN.",
                Data = null
            };
        }

        // Si hay nueva imagen, eliminar la anterior
        if (!string.IsNullOrEmpty(book.ImgUrlLocal) && !string.IsNullOrEmpty(existing.ImgUrlLocal) && existing.ImgUrlLocal != book.ImgUrlLocal && File.Exists(existing.ImgUrlLocal))
        {
            File.Delete(existing.ImgUrlLocal);
        }

        existing.Title = book.Title;
        existing.Description = book.Description;
        existing.Price = book.Price;
        existing.Pages = book.Pages;
        existing.ISBN = book.ISBN;
        existing.Stock = book.Stock;
        existing.YearPublished = book.YearPublished;
        existing.CategoryId = book.CategoryId;
        existing.AuthorId = book.AuthorId;
        existing.PublisherId = book.PublisherId;
        existing.ImgUrl = book.ImgUrl;
        existing.ImgUrlLocal = book.ImgUrlLocal;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var updatedBook = await _db.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher).FirstOrDefaultAsync(b => b.Id == existing.Id);

        return new ApiResponse<BookDto>
        {
            Success = true,
            Message = "Libro actualizado correctamente.",
            Data = updatedBook!.Adapt<BookDto>()
        };
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

    public async Task<ApiResponse<bool>> DeleteBook(int id)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Libro no encontrado.",
                Data = false
            };
        }

        // Eliminar imagen local
        if (!string.IsNullOrEmpty(book.ImgUrlLocal) && File.Exists(book.ImgUrlLocal))
        {
            File.Delete(book.ImgUrlLocal);
        }

        _db.Books.Remove(book);

        var deleted = await _db.SaveChangesAsync() > 0;

        if (!deleted)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al eliminar el libro.",
                Data = false
            };
        }

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Libro eliminado correctamente.",
            Data = true
        };
    }

}