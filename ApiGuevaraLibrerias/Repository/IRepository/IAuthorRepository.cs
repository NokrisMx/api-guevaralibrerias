using ApiGuevaraLibrerias.Models;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAuthors();
    Task<Author?> GetAuthor(int id);
    Task<bool> AuthorExists(int id);
    Task<bool> AuthorExistsByName(string name, int excludeId = 0);
    Task<Author> CreateAuthor(Author author);
    Task<Author?> UpdateAuthor(Author author);
    Task<bool> DeleteAuthor(int id);
}
