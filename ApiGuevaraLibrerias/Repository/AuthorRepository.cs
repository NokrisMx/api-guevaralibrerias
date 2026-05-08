using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Mapster;
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

    public async Task<ApiResponse<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await _db.Authors.OrderBy(a => a.Name).ProjectToType<AuthorDto>().ToListAsync();

        return new ApiResponse<IEnumerable<AuthorDto>>
        {
            Success = true,
            Message = "Autores obtenidos correctamente",
            Data = authors
        };
    }

    public async Task<ApiResponse<AuthorDto>> GetAuthor(int id)
    {
        var author = await _db.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return new ApiResponse<AuthorDto>
            {
                Success = false,
                Message = "Autor no encontrado",
                Data = null
            };
        }

        return new ApiResponse<AuthorDto>
        {
            Success = true,
            Message = "Autor obtenido correctamente",
            Data = author.Adapt<AuthorDto>()
        };
    }

    public async Task<ApiResponse<AuthorDto>> CreateAuthor(Author author)
    {
        if (await AuthorExistsByName(author.Name))
        {
            return new ApiResponse<AuthorDto>
            {
                Success = false,
                Message = "El autor ya existe",
                Data = null
            };
        }

        author.CreatedAt = DateTime.UtcNow;

        await _db.Authors.AddAsync(author);
        await _db.SaveChangesAsync();

        return new ApiResponse<AuthorDto>
        {
            Success = true,
            Message = "Autor creado correctamente",
            Data = author.Adapt<AuthorDto>()
        };
    }


    public async Task<ApiResponse<AuthorDto>> UpdateAuthor(Author author)
    {
        var existingAuthor = await _db.Authors
            .FirstOrDefaultAsync(a => a.Id == author.Id);

        if (existingAuthor == null)
        {
            return new ApiResponse<AuthorDto>
            {
                Success = false,
                Message = "Autor no encontrado",
                Data = null
            };
        }

        if (await AuthorExistsByName(author.Name, author.Id))
        {
            return new ApiResponse<AuthorDto>
            {
                Success = false,
                Message = "El autor ya existe",
                Data = null
            };
        }

        existingAuthor.Name = author.Name;
        existingAuthor.Bio = author.Bio;
        existingAuthor.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ApiResponse<AuthorDto>
        {
            Success = true,
            Message = "Autor actualizado correctamente",
            Data = existingAuthor.Adapt<AuthorDto>()
        };
    }

    public async Task<ApiResponse<bool>> DeleteAuthor(int id)
    {
        var author = await _db.Authors
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Autor no encontrado",
                Data = false
            };
        }

        _db.Authors.Remove(author);

        await _db.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Autor eliminado correctamente",
            Data = true
        };
    }
}
