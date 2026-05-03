using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _db;
    public AuthorRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> AuthorExists(int id)
    {
        return await _db.Authors.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> AuthorExistsByName(string name, int excludeId = 0)
    {
        return await _db.Authors.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim() && c.Id != excludeId);
    }

    public async Task<Author> CreateAuthor(Author author)
    {
        author.CreatedAt = DateTime.UtcNow;
        await _db.Authors.AddAsync(author);
        await _db.SaveChangesAsync();
        return author;
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var author = await _db.Authors.FirstOrDefaultAsync(c => c.Id == id);
        if (author == null)
            return false;
        _db.Authors.Remove(author);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<Author?> GetAuthor(int id)
    {
        return await _db.Authors.Include(a => a.Books).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Author>> GetAuthors()
    {
        return await _db.Authors.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Author?> UpdateAuthor(Author author)
    {
        var existingAuthor = await _db.Authors.FirstOrDefaultAsync(c => c.Id == author.Id);
        if (existingAuthor == null)
            return null;
        // Actualizar solo campos necesarios
        existingAuthor.Name = author.Name;
        existingAuthor.Bio = author.Bio;
        existingAuthor.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return existingAuthor;
    }
}
