using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiGuevaraLibrerias.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsers()
    {
        var users = _userManager.Users.ToList();

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserDto
            {
                Id = user.Id,
                Name = user.Name ?? "",
                Username = user.UserName ?? "",
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "User"
            });
        }
        return new ApiResponse<IEnumerable<UserDto>>
        {
            Success = true,
            Message = "Usuarios obtenidos correctamente",
            Data = result
        };
    }

    public async Task<ApiResponse<UserDto>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Usuario no encontrado",
                Data = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name ?? "",
            Username = user.UserName ?? "",
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Role = roles.FirstOrDefault() ?? "User"
        };

        return new ApiResponse<UserDto>
        {
            Success = true,
            Message = "Usuario obtenido correctamente",
            Data = userDto
        };
    }

    public async Task<bool> UserExists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<ApiResponse<UserDto>> Register(UserRegisterDto dto)
    {
        var existingEmail = await _userManager.FindByEmailAsync(dto.Email);

        if (existingEmail != null)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "El correo electrónico ya está registrado",
                Data = null
            };
        }

        var existingUsername = await _userManager.FindByNameAsync(dto.Username);

        if (existingUsername != null)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "El nombre de usuario ya está en uso",
                Data = null
            };
        }

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Name = dto.Name,
            NormalizedEmail = dto.Email.ToUpper()
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = result.Errors.FirstOrDefault()?.Description
                    ?? "No se pudo registrar el usuario",
                Data = null
            };
        }

        // Crear rol si no existe
        if (!await _roleManager.RoleExistsAsync(dto.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole(dto.Role));
        }

        // Asignar rol
        await _userManager.AddToRoleAsync(user, dto.Role);

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name ?? string.Empty,
            Username = user.UserName ?? "",
            Email = user.Email ?? "",
            Role = dto.Role
        };

        return new ApiResponse<UserDto>
        {
            Success = true,
            Message = "Usuario registrado correctamente",
            Data = userDto
        };
    }

    public async Task<ApiResponse<AuthResponseDto>> Login(UserLoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Correo o contraseña incorrectos",
                Data = null
            };
        }

        var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isValid)
        {
            return new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Message = "Correo o contraseña incorrectos",
                Data = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault() ?? "User";

        var token = GenerateJwtToken(user, role);

        var authResponse = new AuthResponseDto
        {
            Id = user.Id,
            Token = token,
            Email = user.Email!,
            Name = user.Name,
            Username = user.UserName!,
            PhoneNumber = user.PhoneNumber,
            Role = role
        };
        return new ApiResponse<AuthResponseDto>
        {
            Success = true,
            Message = "Inicio de sesión exitoso",
            Data = authResponse
        };
    }

    public async Task<ApiResponse<UserDto>> UpdateUser(string userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Usuario no encontrado",
                Data = null
            };
        }

        if (!string.IsNullOrWhiteSpace(dto.Username))
        {
            var existingUsername = await _userManager.FindByNameAsync(dto.Username);

            if (existingUsername != null && existingUsername.Id != user.Id)
            {
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "El nombre de usuario ya está en uso",
                    Data = null
                };
            }

            user.UserName = dto.Username;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);

            if (existingEmail != null && existingEmail.Id != user.Id)
            {
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "El correo electrónico ya está registrado",
                    Data = null
                };
            }

            user.Email = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpper();
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            var existingPhone = await _userManager.Users
                .FirstOrDefaultAsync(u =>
                    u.PhoneNumber == dto.PhoneNumber &&
                    u.Id != user.Id);

            if (existingPhone != null)
            {
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "El número telefónico ya está registrado",
                    Data = null
                };
            }

            user.PhoneNumber = dto.PhoneNumber;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return new ApiResponse<UserDto>
            {
                Success = false,
                Message = "No se pudo actualizar el usuario",
                Data = null
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new ApiResponse<UserDto>
        {
            Success = true,
            Message = "Perfil actualizado correctamente",
            Data = new UserDto
            {
                Id = user.Id,
                Name = user.Name ?? "",
                Username = user.UserName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "User"
            }
        };
    }

    private string GenerateJwtToken(ApplicationUser user, string role)
    {
        var secretKey = _configuration.GetValue<string>("ApiSettings:SecretKey")!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Name, user.Name ?? user.UserName!)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
