using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IPublisherRepository
{
    Task<ApiResponse<IEnumerable<PublisherDto>>> GetPublishers();
    Task<ApiResponse<PublisherDto>> GetPublisher(int id);
    Task<bool> PublisherExists(int id);
    Task<bool> PublisherExistsByName(string name, int excludeId = 0);
    Task<ApiResponse<PublisherDto>> CreatePublisher(Publisher publisher);
    Task<ApiResponse<PublisherDto>> UpdatePublisher(Publisher publisher);
    Task<ApiResponse<bool>> DeletePublisher(int id);

}
