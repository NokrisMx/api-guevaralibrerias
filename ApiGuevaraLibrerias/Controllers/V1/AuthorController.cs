using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGuevaraLibrerias.Controllers.V1
{
    [Authorize(Roles = "Admin")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _authorRepository.GetAuthors();
            return Ok(authors.Adapt<IEnumerable<AuthorDto>>());
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que 0");

            var author = await _authorRepository.GetAuthor(id);
            if (author == null)
                return NotFound($"El autor con ID {id} no fue encontrado");

            return Ok(author.Adapt<AuthorDetailDto>());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _authorRepository.AuthorExistsByName(dto.Name))
                return Conflict($"El autor '{dto.Name}' ya existe");

            var created = await _authorRepository.CreateAuthor(dto.Adapt<Author>());

            return CreatedAtRoute("GetAuthor", new { id = created.Id }, created.Adapt<AuthorDto>());
        }

        [HttpPut("{id:int}", Name = "UpdateAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto dto)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _authorRepository.AuthorExistsByName(dto.Name, excludeId: id))
                return Conflict($"El autor '{dto.Name}' ya existe");

            var author = new Author { Id = id, Name = dto.Name, Bio = dto.Bio };
            var updated = await _authorRepository.UpdateAuthor(author);

            if (updated == null)
                return NotFound($"El autor con ID {id} no fue encontrado");

            return Ok(updated.Adapt<AuthorDto>());
        }

        [HttpDelete("{id:int}", Name = "DeleteAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que 0");

            if (!await _authorRepository.AuthorExists(id))
                return NotFound($"El autor con ID {id} no fue encontrado");

            var deleted = await _authorRepository.DeleteAuthor(id);

            if (!deleted)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar el autor");

            return Ok(new { message = $"El autor con ID {id} fue eliminado correctamente" });
        }


    }
}
