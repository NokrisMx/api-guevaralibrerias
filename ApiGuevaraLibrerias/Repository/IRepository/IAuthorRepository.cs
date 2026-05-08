using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IAuthorRepository
{
    Task<ApiResponse<IEnumerable<AuthorDto>>> GetAuthors();
    Task<ApiResponse<AuthorDto>> GetAuthor(int id);
    Task<bool> AuthorExists(int id);
    Task<bool> AuthorExistsByName(string name, int excludeId = 0);
    Task<ApiResponse<AuthorDto>> CreateAuthor(Author author);
    Task<ApiResponse<AuthorDto>> UpdateAuthor(Author author);
    Task<ApiResponse<bool>> DeleteAuthor(int id);
}
