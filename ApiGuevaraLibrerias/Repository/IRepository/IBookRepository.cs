using ApiGuevaraLibrerias.Models;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooks();
    Task<Book?> GetBook(int id);
    Task<bool> BookExists(int id);
    Task<bool> BookExistsByISBN(string isbn, int excludeId = 0);
    Task<Book> CreateBook(Book book);
    Task<Book?> UpdateBook(Book book);
    Task<bool> UpdateStock(int bookId, int newStock);
    Task<bool> DeleteBook(int id);
}