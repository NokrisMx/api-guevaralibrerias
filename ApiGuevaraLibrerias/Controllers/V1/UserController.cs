using System.Security.Claims;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGuevaraLibrerias.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null)
                return NotFound($"El usuario con ID {id} no fue encontrado.");

            return Ok(user);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var response = await _userRepository.Register(dto);

            if (!response.Success)
                return BadRequest(response);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var response = await _userRepository.Login(dto);

            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Usuario no autenticado."
                });
            }

            var response = await _userRepository.UpdateUser(userId, dto);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [Authorize]
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuario no autenticado.");

            var result = await _userRepository.ChangePassword(userId, dto);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al cambiar la contraseña.",
                    Data = false,
                });

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Contraseña actualizada correctamente.",
                Data = true,
            });
        }
    }
}
