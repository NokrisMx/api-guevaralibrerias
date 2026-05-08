using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface ICategoryRepository
{
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategories();
    Task<ApiResponse<CategoryDto>> GetCategory(int id);
    Task<bool> CategoryExists(int id);
    Task<bool> CategoryExistsByName(string name, int excludeId = 0);
    Task<ApiResponse<CategoryDto>> CreateCategory(Category category);
    Task<ApiResponse<CategoryDto>> UpdateCategory(Category category);
    Task<ApiResponse<bool>> DeleteCategory(int id);
}