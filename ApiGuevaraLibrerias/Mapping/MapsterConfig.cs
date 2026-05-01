using ApiGuevaraLibrerias.Models;
using ApiGuevaraLibrerias.Models.Dtos;
using Mapster;

namespace ApiGuevaraLibrerias.Mapping;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        // CATEGORY
        TypeAdapterConfig<Category, CategoryDto>.NewConfig().TwoWays();
        TypeAdapterConfig<Category, CreateCategoryDto>.NewConfig().TwoWays();

        // AUTHOR
        TypeAdapterConfig<Author, AuthorDto>.NewConfig().TwoWays();
        TypeAdapterConfig<Author, CreateAuthorDto>.NewConfig().TwoWays();

        // BOOK
        TypeAdapterConfig<Book, BookDto>.NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.AuthorName, src => src.Author.Name);
        TypeAdapterConfig<Book, CreateBookDto>.NewConfig().TwoWays();
        TypeAdapterConfig<Book, UpdateBookDto>.NewConfig().TwoWays();

        // ORDER DETAIL
        TypeAdapterConfig<OrderDetail, OrderDetailDto>.NewConfig()
            .Map(dest => dest.BookId, src => src.BookId)
            .Map(dest => dest.Title, src => src.Book.Title)
            .Map(dest => dest.SubTotal, src => src.Price * src.Quantity);

        // ORDER
        TypeAdapterConfig<Order, OrderDto>.NewConfig()
            .Map(dest => dest.Username, src => src.User.UserName)
            .Map(dest => dest.Items, src => src.OrderDetails)
            .Map(dest => dest.Status, src => src.Status.ToString());
    }
}