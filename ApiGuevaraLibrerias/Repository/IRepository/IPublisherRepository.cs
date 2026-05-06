using ApiGuevaraLibrerias.Models;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IPublisherRepository
{
    Task<IEnumerable<Publisher>> GetPublishers();
    Task<Publisher?> GetPublisher(int id);
    Task<bool> PublisherExists(int id);
    Task<bool> PublisherExistsByName(string name, int excludeId = 0);
    Task<Publisher> CreatePublisher(Publisher publisher);
    Task<Publisher?> UpdatePublisher(Publisher publisher);
    Task<bool> DeletePublisher(int id);

}
