using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using ApiGuevaraLibrerias.Models.Responses;
using ApiGuevaraLibrerias.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    private readonly IPublisherRepository _publisherRepository;

    public BookController(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IAuthorRepository authorRepository, IPublisherRepository publisherRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _authorRepository = authorRepository;
        _publisherRepository = publisherRepository;
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetBook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBook(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ApiResponse<BookDto>
            {
                Success = false,
                Message = "El ID debe ser mayor que 0",
                Data = null
            });
        }

        var response = await _bookRepository.GetBook(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBookInPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? query = null, [FromQuery] int? categoryId = null, [FromQuery] int? authorId = null, [FromQuery] int? publisherId = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "La página y el tamaño deben ser mayores que 0.",
                Data = null
            });
        }

        var booksQuery = _bookRepository.GetBooksQuery();

        // BÚSQUEDA
        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim().ToLower();

            booksQuery = booksQuery.Where(b =>
                EF.Functions.Collate(b.Title, "SQL_Latin1_General_CP1_CI_AI").Contains(query)
            );
        }

        // FILTROS
        if (categoryId.HasValue && categoryId > 0)
            booksQuery = booksQuery.Where(b => b.CategoryId == categoryId.Value);

        if (authorId.HasValue && authorId > 0)
            booksQuery = booksQuery.Where(b => b.AuthorId == authorId.Value);

        if (publisherId.HasValue && publisherId > 0)
            booksQuery = booksQuery.Where(b => b.PublisherId == publisherId.Value);

        if (minPrice.HasValue)
            booksQuery = booksQuery.Where(b => b.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            booksQuery = booksQuery.Where(b => b.Price <= maxPrice.Value);

        // PAGINACIÓN
        var totalRecords = await booksQuery.CountAsync();

        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pagedBooks = await booksQuery.Skip((page - 1) * pageSize).Take(pageSize).ProjectToType<BookDto>().ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Libros obtenidos correctamente",
            Data = new
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Data = pagedBooks
            }
        });
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

        if (!await _categoryRepository.CategoryExists(dto.CategoryId))
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = $"La categoría con ID {dto.CategoryId} no fue encontrada.",
                Data = null
            });
        }

        if (!await _authorRepository.AuthorExists(dto.AuthorId))
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = $"El autor con ID {dto.AuthorId} no fue encontrado.",
                Data = null
            });
        }

        if (!await _publisherRepository.PublisherExists(dto.PublisherId))
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = $"La editorial con ID {dto.PublisherId} no fue encontrada.",
                Data = null
            });
        }


        var book = new Book
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Pages = dto.Pages,
            ISBN = dto.ISBN,
            Stock = dto.Stock,
            YearPublished = dto.YearPublished,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId,
            PublisherId = dto.PublisherId,
            ImgUrl = dto.ImgUrl
        };

        // Guardar primero para obtener el Id generado
        var response = await _bookRepository.CreateBook(book);

        if (!response.Success)
            return Conflict(response);

        // Subir imagen 
        UploadBookImage(dto, book);

        // Actualizar URLs si se subió imagen
        if (dto.Image != null)
            await _bookRepository.UpdateBook(book);

        // Obtener libro actualizado
        var updatedBook = await _bookRepository.GetBook(book.Id);

        response.Data = updatedBook.Data;

        return CreatedAtRoute(
        "GetBook",
        new { id = book.Id },
        response
    );
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
        {
            return BadRequest(new ApiResponse<BookDto>
            {
                Success = false,
                Message = "ID inválido",
                Data = null
            });
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _categoryRepository.CategoryExists(dto.CategoryId))
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = $"La categoría con ID {dto.CategoryId} no fue encontrada.",
                Data = null
            });
        }

        if (!await _authorRepository.AuthorExists(dto.AuthorId))
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = $"El autor con ID {dto.AuthorId} no fue encontrado.",
                Data = null
            });
        }

        if (!await _publisherRepository.PublisherExists(dto.PublisherId))
        {
            return NotFound(new ApiResponse<BookDto>
            {
                Success = false,
                Message = $"La editorial con ID {dto.PublisherId} no fue encontrada.",
                Data = null
            });
        }

        var book = new Book
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Pages = dto.Pages,
            ISBN = dto.ISBN,
            Stock = dto.Stock,
            YearPublished = dto.YearPublished,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId,
            PublisherId = dto.PublisherId,
            ImgUrl = dto.ImgUrl,
            ImgUrlLocal = dto.ImgUrlLocal
        };

        // Subir nueva imagen si se envió
        UploadBookImage(dto, book);

        var response = await _bookRepository.UpdateBook(book);

        if (!response.Success)
        {
            if (response.Message.Contains("ISBN"))
                return Conflict(response);

            return NotFound(response);
        }

        return Ok(response);
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
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "El ID debe ser mayor que 0.",
                Data = false
            });
        }

        var response = await _bookRepository.DeleteBook(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
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