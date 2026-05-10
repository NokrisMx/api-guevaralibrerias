using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IBookRepository
{
    Task<ApiResponse<BookDto>> GetBook(int id);
    IQueryable<Book> GetBooksQuery();
    Task<bool> BookExists(int id);
    Task<bool> BookExistsByISBN(string isbn, int excludeId = 0);
    Task<ApiResponse<BookDto>> CreateBook(Book book);
    Task<ApiResponse<BookDto>> UpdateBook(Book book);
    Task<bool> UpdateStock(int bookId, int newStock);
    Task<ApiResponse<bool>> DeleteBook(int id);
}