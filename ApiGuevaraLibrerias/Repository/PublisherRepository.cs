using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class PublisherRepository : IPublisherRepository
{
    private readonly ApplicationDbContext _db;
    public PublisherRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<bool> PublisherExists(int id)
    {
        return await _db.Publishers.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> PublisherExistsByName(string name, int excludeId = 0)
    {
        return await _db.Publishers.AnyAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim() && p.Id != excludeId);

    }

    public async Task<ApiResponse<PublisherDto>> GetPublisher(int id)
    {
        var publisher = await _db.Publishers.FirstOrDefaultAsync(p => p.Id == id);

        if (publisher == null)
        {
            return new ApiResponse<PublisherDto>
            {
                Success = false,
                Message = "Editorial no encontrada.",
                Data = null
            };
        }

        return new ApiResponse<PublisherDto>
        {
            Success = true,
            Message = "Editorial obtenida correctamente.",
            Data = publisher.Adapt<PublisherDto>()
        };
    }

    public async Task<ApiResponse<IEnumerable<PublisherDto>>> GetPublishers()
    {
        var publishers = await _db.Publishers.OrderBy(p => p.Name).ProjectToType<PublisherDto>().ToListAsync();

        return new ApiResponse<IEnumerable<PublisherDto>>
        {
            Success = true,
            Message = "Editoriales obtenidas correctamente.",
            Data = publishers
        };
    }

    public async Task<ApiResponse<PublisherDto>> CreatePublisher(Publisher publisher)
    {
        if (await PublisherExistsByName(publisher.Name))
        {
            return new ApiResponse<PublisherDto>
            {
                Success = false,
                Message = "La editorial ya existe.",
                Data = null
            };
        }

        publisher.CreatedAt = DateTime.UtcNow;

        await _db.Publishers.AddAsync(publisher);
        await _db.SaveChangesAsync();

        return new ApiResponse<PublisherDto>
        {
            Success = true,
            Message = "Editorial creada correctamente.",
            Data = publisher.Adapt<PublisherDto>()
        };
    }

    public async Task<ApiResponse<PublisherDto>> UpdatePublisher(Publisher publisher)
    {
        var existingPublisher = await _db.Publishers.FirstOrDefaultAsync(p => p.Id == publisher.Id);

        if (existingPublisher == null)
        {
            return new ApiResponse<PublisherDto>
            {
                Success = false,
                Message = "Editorial no encontrada.",
                Data = null
            };
        }

        if (await PublisherExistsByName(publisher.Name, publisher.Id))
        {
            return new ApiResponse<PublisherDto>
            {
                Success = false,
                Message = "La editorial ya existe.",
                Data = null
            };
        }

        existingPublisher.Name = publisher.Name;
        existingPublisher.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ApiResponse<PublisherDto>
        {
            Success = true,
            Message = "Editorial actualizada correctamente.",
            Data = existingPublisher.Adapt<PublisherDto>()
        };
    }

    public async Task<ApiResponse<bool>> DeletePublisher(int id)
    {
        var publisher = await _db.Publishers.FirstOrDefaultAsync(p => p.Id == id);

        if (publisher == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Editorial no encontrada.",
                Data = false
            };
        }

        _db.Publishers.Remove(publisher);

        await _db.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Editorial eliminada correctamente.",
            Data = true
        };
    }
}
