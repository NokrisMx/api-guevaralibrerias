using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
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

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.UserExists(dto.Email))
                return Conflict($"El correo '{dto.Email}' ya está registrado");

            var user = await _userRepository.Register(dto);

            if (user == null)
                return BadRequest("Error al registrar el usuario");

            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userRepository.Login(dto);

            if (response == null)
                return Unauthorized("Correo o contraseña incorrectos");

            return Ok(response);
        }
    }
}
