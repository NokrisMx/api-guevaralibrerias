using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiGuevaraLibrerias.Repository;

public class PublisherRepository : IPublisherRepository
{
    private readonly ApplicationDbContext _db;
    public PublisherRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<Publisher> CreatePublisher(Publisher publisher)
    {
        publisher.CreatedAt = DateTime.UtcNow;
        await _db.Publishers.AddAsync(publisher);
        await _db.SaveChangesAsync();
        return publisher;
    }

    public async Task<bool> DeletePublisher(int id)
    {
        var publisher = await _db.Publishers.FirstOrDefaultAsync(p => p.Id == id);
        if (publisher == null)
            return false;
        _db.Publishers.Remove(publisher);
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<Publisher?> GetPublisher(int id)
    {
        return await _db.Publishers.FirstOrDefaultAsync(p => p.Id == id);

    }

    public async Task<IEnumerable<Publisher>> GetPublishers()
    {
        return await _db.Publishers.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<bool> PublisherExists(int id)
    {
        return await _db.Publishers.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> PublisherExistsByName(string name, int excludeId = 0)
    {
        return await _db.Publishers.AnyAsync(p => p.Name.ToLower().Trim() == name.ToLower().Trim() && p.Id != excludeId);

    }

    public Task<Publisher?> UpdatePublisher(Publisher publisher)
    {
        throw new NotImplementedException();
    }
}
