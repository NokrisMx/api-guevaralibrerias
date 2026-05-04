using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
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

    public async Task<IEnumerable<UserDto>> GetUsers()
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
        return result;
    }

    public async Task<UserDto?> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name ?? "",
            Username = user.UserName ?? "",
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Role = roles.FirstOrDefault() ?? "User"
        };
    }

    public async Task<bool> UserExists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<UserDto?> Register(UserRegisterDto dto)
    {
        if (await UserExists(dto.Email))
            return null;

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Name = dto.Name,
            NormalizedEmail = dto.Email.ToUpper()
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return null;

        // Crear el rol si no existe
        if (!await _roleManager.RoleExistsAsync(dto.Role))
            await _roleManager.CreateAsync(new IdentityRole(dto.Role));

        // Asignar el rol al usuario
        await _userManager.AddToRoleAsync(user, dto.Role);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name ?? string.Empty,
            Username = user.UserName,
            Email = user.Email,
            Role = dto.Role
        };
    }

    public async Task<AuthResponseDto?> Login(UserLoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return null;

        var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValid)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        var token = GenerateJwtToken(user, role);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Name = user.Name,
            Username = user.UserName!,
            Role = role
        };
    }

    public async Task<UserDto?> UpdateUser(string userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Name))
            user.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Username))
            user.UserName = dto.Username;

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        // Email necesita actualizarse con SetEmailAsync porque Identity
        // maneja NormalizedEmail y tokens de confirmación internamente
        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
            if (!emailResult.Succeeded)
                return null;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name ?? "",
            Username = user.UserName ?? "",
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Role = roles.FirstOrDefault() ?? "User"
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
