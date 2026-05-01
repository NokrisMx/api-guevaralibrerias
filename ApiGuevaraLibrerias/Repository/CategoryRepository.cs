using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Repository.IRepository;
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

    public async Task<bool> CategoryExistsByName(string name)
    {
        return await _db.Categories.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public async Task<Category> CreateCategory(Category category)
    {
        category.CreatedAt = DateTime.UtcNow;
        await _db.Categories.AddAsync(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategory(int id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return false;
        _db.Categories.Remove(category);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category?> GetCategory(int id)
    {
        return await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> UpdateCategory(Category category)
    {
        var existingCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
        if (existingCategory == null)
            return null;
        // Actualizar solo campos necesarios
        existingCategory.Name = category.Name;
        existingCategory.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return existingCategory;
    }
}
