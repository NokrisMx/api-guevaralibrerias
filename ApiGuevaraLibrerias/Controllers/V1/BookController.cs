using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGuevaraLibrerias.Controllers.V1;

[Authorize(Roles = "Admin")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuthorRepository _authorRepository;

    public BookController(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _authorRepository = authorRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _bookRepository.GetBooks();
        return Ok(books.Adapt<IEnumerable<BookDto>>());
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetBook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBook(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor que 0");

        var book = await _bookRepository.GetBook(id);
        if (book == null)
            return NotFound($"El libro con ID {id} no fue encontrado");

        return Ok(book.Adapt<BookDto>());
    }

    [AllowAnonymous]
    [HttpGet("page")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBookInPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null, [FromQuery] int? authorId = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("La página y el tamaño deben ser mayores que 0");

        var books = await _bookRepository.GetBooks();
        var query = books.AsQueryable();

        // Aplicar filtros
        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        if (minPrice.HasValue)
            query = query.Where(b => b.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(b => b.Price <= maxPrice.Value);

        var totalRecords = query.Count();
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pagedBooks = query.Skip((page - 1) * pageSize).Take(pageSize).Adapt<IEnumerable<BookDto>>();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            Data = pagedBooks
        });
    }

    [AllowAnonymous]
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchBooks([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("El término de búsqueda es obligatorio");

        var books = await _bookRepository.GetBooks();

        var result = books
            .Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Adapt<IEnumerable<BookDto>>();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBook([FromForm] CreateBookDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _bookRepository.BookExistsByISBN(dto.ISBN))
            return Conflict($"Ya existe un libro con el ISBN '{dto.ISBN}'");

        if (!await _categoryRepository.CategoryExists(dto.CategoryId))
            return NotFound($"La categoría con ID {dto.CategoryId} no fue encontrada");

        if (!await _authorRepository.AuthorExists(dto.AuthorId))
            return NotFound($"El autor con ID {dto.AuthorId} no fue encontrado");

        var book = new Book
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            ISBN = dto.ISBN,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId,
            ImgUrl = dto.ImgUrl
        };

        // Guardar primero para obtener el Id generado
        var created = await _bookRepository.CreateBook(book);

        // Subir imagen después de tener el Id
        UploadBookImage(dto, created);

        // Si se subió imagen, actualizar el libro con las URLs
        if (dto.Image != null)
            await _bookRepository.UpdateBook(created);

        // Traer el libro completo con Author y Category
        var bookWithRelations = await _bookRepository.GetBook(created.Id);

        return CreatedAtRoute("GetBook", new { id = created.Id }, bookWithRelations!.Adapt<BookDto>());
    }

    [HttpPut("{id:int}", Name = "UpdateBook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBook(int id, [FromForm] UpdateBookDto dto)
    {
        if (id <= 0)
            return BadRequest("ID inválido");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _bookRepository.BookExistsByISBN(dto.ISBN, excludeId: id))
            return Conflict($"Ya existe un libro con el ISBN '{dto.ISBN}'");

        if (!await _categoryRepository.CategoryExists(dto.CategoryId))
            return NotFound($"La categoría con ID {dto.CategoryId} no fue encontrada");

        if (!await _authorRepository.AuthorExists(dto.AuthorId))
            return NotFound($"El autor con ID {dto.AuthorId} no fue encontrado");

        var book = new Book
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            ISBN = dto.ISBN,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId,
            ImgUrl = dto.ImgUrl,
            ImgUrlLocal = dto.ImgUrlLocal
        };
        book.Id = id;

        // Subir nueva imagen si se envió
        UploadBookImage(dto, book);

        var updated = await _bookRepository.UpdateBook(book);
        if (updated == null)
            return NotFound($"El libro con ID {id} no fue encontrado");

        // Traer el libro completo con Author y Category
        var bookWithRelations = await _bookRepository.GetBook(updated.Id);

        return Ok(bookWithRelations!.Adapt<BookDto>());
    }

    [HttpDelete("{id:int}", Name = "DeleteBook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor que 0");

        if (!await _bookRepository.BookExists(id))
            return NotFound($"El libro con ID {id} no fue encontrado");

        var deleted = await _bookRepository.DeleteBook(id);
        if (!deleted)
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar el libro");

        return Ok(new { message = $"El libro con ID {id} fue eliminado correctamente" });
    }

    // Método privado para subir imagen
    private void UploadBookImage(dynamic dto, Book book)
    {
        if (dto.Image != null)
        {
            string fileName = book.Id + Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BookImages");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var filePath = Path.Combine(imagesFolder, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            dto.Image.CopyTo(fileStream);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            book.ImgUrl = $"{baseUrl}/BookImages/{fileName}";
            book.ImgUrlLocal = filePath;
        }
        else
        {
            // Solo asigna placeholder si no tiene URL previa
            if (string.IsNullOrEmpty(book.ImgUrl))
                book.ImgUrl = "https://placehold.co/300x300";
        }
    }
}