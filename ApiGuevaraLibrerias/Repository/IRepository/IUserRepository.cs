using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace ApiGuevaraLibrerias.Repository.IRepository;

public interface IUserRepository
{
    Task<ApiResponse<IEnumerable<UserDto>>> GetUsers();
    Task<ApiResponse<UserDto>> GetUser(string id);
    Task<bool> UserExists(string email);
    Task<ApiResponse<UserDto>> Register(UserRegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> Login(UserLoginDto dto);
    Task<ApiResponse<AuthResponseDto>> GoogleLogin(GoogleLoginDto dto);
    Task<ApiResponse<UserDto>> UpdateUser(string userId, UpdateUserDto dto);
    Task<IdentityResult> ChangePassword(string userId, ChangePasswordDto dto);
}
