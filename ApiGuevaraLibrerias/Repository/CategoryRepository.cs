using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<bool> CategoryExists(int id)
    {
        return await _db.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> CategoryExistsByName(string name, int excludeId = 0)
    {
        return await _db.Categories.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim() && c.Id != excludeId);
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ProjectToType<CategoryDto>().ToListAsync();

        return new ApiResponse<IEnumerable<CategoryDto>>
        {
            Success = true,
            Message = "Categorías obtenidas correctamente.",
            Data = categories
        };
    }

    public async Task<ApiResponse<CategoryDto>> GetCategory(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return new ApiResponse<CategoryDto>
            {
                Success = false,
                Message = "Categoría no encontrada.",
                Data = null
            };
        }

        return new ApiResponse<CategoryDto>
        {
            Success = true,
            Message = "Categoría obtenida correctamente.",
            Data = category.Adapt<CategoryDto>()
        };
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategory(Category category)
    {
        if (await CategoryExistsByName(category.Name))
        {
            return new ApiResponse<CategoryDto>
            {
                Success = false,
                Message = "La categoría ya existe.",
                Data = null
            };
        }

        category.CreatedAt = DateTime.UtcNow;

        await _db.Categories.AddAsync(category);
        await _db.SaveChangesAsync();

        return new ApiResponse<CategoryDto>
        {
            Success = true,
            Message = "Categoría creada correctamente.",
            Data = category.Adapt<CategoryDto>()
        };
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategory(Category category)
    {
        var existingCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);

        if (existingCategory == null)
        {
            return new ApiResponse<CategoryDto>
            {
                Success = false,
                Message = "Categoría no encontrada.",
                Data = null
            };
        }

        if (await CategoryExistsByName(category.Name, category.Id))
        {
            return new ApiResponse<CategoryDto>
            {
                Success = false,
                Message = "La categoría ya existe.",
                Data = null
            };
        }

        existingCategory.Name = category.Name;
        existingCategory.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ApiResponse<CategoryDto>
        {
            Success = true,
            Message = "Categoría actualizada correctamente.",
            Data = existingCategory.Adapt<CategoryDto>()
        };
    }

    public async Task<ApiResponse<bool>> DeleteCategory(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Categoría no encontrada.",
                Data = false
            };
        }

        _db.Categories.Remove(category);

        await _db.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Categoría eliminada correctamente.",
            Data = true
        };
    }
}

