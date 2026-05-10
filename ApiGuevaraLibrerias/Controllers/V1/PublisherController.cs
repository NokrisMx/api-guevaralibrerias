using ApiGuevaraLibrerias.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using ApiGuevaraLibrerias.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Asp.Versioning;
using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Responses;

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
            var response = await _publisherRepository.GetPublishers();

            return Ok(response);
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
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser mayor que 0."
                });
            }

            var response = await _publisherRepository.GetPublisher(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
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

            var response = await _publisherRepository
                .CreatePublisher(dto.Adapt<Publisher>());

            if (!response.Success)
                return Conflict(response);

            return CreatedAtRoute(
                "GetPublisher",
                new { id = response.Data!.Id },
                response
            );
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
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID inválido."
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var publisher = new Publisher
            {
                Id = id,
                Name = dto.Name
            };

            var response = await _publisherRepository.UpdatePublisher(publisher);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
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
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "El ID debe ser mayor que 0."
                });
            }

            var response = await _publisherRepository.DeletePublisher(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

    }
}
