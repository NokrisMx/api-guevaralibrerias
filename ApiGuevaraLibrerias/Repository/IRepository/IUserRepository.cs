using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IUserRepository
{
    Task<IEnumerable<UserDto>> GetUsers();
    Task<UserDto?> GetUser(string id);
    Task<bool> UserExists(string email);
    Task<UserDto?> Register(UserRegisterDto dto);
    Task<AuthResponseDto?> Login(UserLoginDto dto);
    Task<UserDto?> UpdateUser(string userId, UpdateUserDto dto);
}
