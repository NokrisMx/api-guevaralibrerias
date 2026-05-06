using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using ApiGuevaraLibrerias.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Asp.Versioning;
using ApiGuevaraLibrerias.Models;

namespace ApiGuevaraLibrerias.Controllers.V1
{
    [Authorize(Roles = "Admin")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublishers()
        {
            var publishers = await _publisherRepository.GetPublishers();
            var publisherDtos = publishers.Adapt<IEnumerable<PublisherDto>>();
            return Ok(publisherDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetPublisher")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublisher(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que 0");
            var publisher = await _publisherRepository.GetPublisher(id);
            if (publisher == null)
                return NotFound($"La editorial con ID {id} no fue encontrada");

            var publisherDto = publisher.Adapt<PublisherDto>();
            return Ok(publisherDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _publisherRepository.PublisherExistsByName(dto.Name))
                return Conflict($"La editorial '{dto.Name}' ya existe");

            var created = await _publisherRepository.CreatePublisher(dto.Adapt<Publisher>());

            return CreatedAtRoute("GetPublisher", new { id = created.Id }, created.Adapt<PublisherDto>());
        }

        [HttpPut("{id:int}", Name = "UpdatePublisher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePublisher(int id, [FromBody] UpdatePublisherDto dto)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Se excluye el ID actual para permitir guardar con el mismo nombre
            if (await _publisherRepository.PublisherExistsByName(dto.Name, excludeId: id))
                return Conflict($"La editorial '{dto.Name}' ya existe");

            var publisher = new Publisher { Id = id, Name = dto.Name };
            var updated = await _publisherRepository.UpdatePublisher(publisher);

            if (updated == null)
                return NotFound($"La editorial con ID {id} no fue encontrada");

            return Ok(updated.Adapt<PublisherDto>());
        }

        [HttpDelete("{id:int}", Name = "DeletePublisher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que 0");

            if (!await _publisherRepository.PublisherExists(id))
                return NotFound($"La editorial con ID {id} no fue encontrada");

            var deleted = await _publisherRepository.DeletePublisher(id);

            if (!deleted)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la editorial");

            return Ok(new { message = $"La editorial con ID {id} fue eliminada correctamente" });
        }

    }
}
