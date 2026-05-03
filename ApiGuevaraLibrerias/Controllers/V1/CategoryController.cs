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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepository.GetCategories();
            var categoryDtos = categories.Adapt<IEnumerable<CategoryDto>>();
            return Ok(categoryDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategory(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que 0");
            var category = await _categoryRepository.GetCategory(id);
            if (category == null)
                return NotFound($"La categoría con ID {id} no fue encontrada");

            var categoryDto = category.Adapt<CategoryDto>();
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _categoryRepository.CategoryExistsByName(dto.Name))
                return Conflict($"La categoría '{dto.Name}' ya existe");

            var created = await _categoryRepository.CreateCategory(dto.Adapt<Category>());

            return CreatedAtRoute("GetCategory", new { id = created.Id }, created.Adapt<CategoryDto>());
        }

        [HttpPut("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Se excluye el ID actual para permitir guardar con el mismo nombre
            if (await _categoryRepository.CategoryExistsByName(dto.Name, excludeId: id))
                return Conflict($"La categoría '{dto.Name}' ya existe");

            var category = new Category { Id = id, Name = dto.Name };
            var updated = await _categoryRepository.UpdateCategory(category);

            if (updated == null)
                return NotFound($"La categoría con ID {id} no fue encontrada");

            return Ok(updated.Adapt<CategoryDto>());
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor que 0");

            if (!await _categoryRepository.CategoryExists(id))
                return NotFound($"La categoría con ID {id} no fue encontrada");

            var deleted = await _categoryRepository.DeleteCategory(id);

            if (!deleted)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la categoría");

            return Ok(new { message = $"La categoría con ID {id} fue eliminada correctamente" });
        }


    }
}
