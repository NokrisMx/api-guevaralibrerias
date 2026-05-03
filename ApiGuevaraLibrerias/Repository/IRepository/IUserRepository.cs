using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IUserRepository
{
    Task<bool> UserExists(string email);
    Task<UserDto?> Register(UserRegisterDto dto);
    Task<AuthResponseDto?> Login(UserLoginDto dto);
}
