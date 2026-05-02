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

        [AllowAnonymous]

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _categoryRepository.CategoryExistsByName(dto.Name);

            if (exists)
                return Conflict($"La categoría '{dto.Name}' ya existe");

            var category = dto.Adapt<Category>();

            var createdCategory = await _categoryRepository.CreateCategory(category);

            var categoryDto = createdCategory.Adapt<CategoryDto>();

            return CreatedAtRoute("GetCategory", new { id = categoryDto.Id }, categoryDto);
        }



    }
}
