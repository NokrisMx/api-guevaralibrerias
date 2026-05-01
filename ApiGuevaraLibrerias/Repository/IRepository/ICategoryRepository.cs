using ApiGuevaraLibrerias.Models;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories();
    Task<Category?> GetCategory(int id);
    Task<bool> CategoryExists(int id);
    Task<bool> CategoryExistsByName(string name);
    Task<Category> CreateCategory(Category category);
    Task<Category?> UpdateCategory(Category category);
    Task<bool> DeleteCategory(int id);
}