using ApiGuevaraLibrerias.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext appContext)
    {
        // Evitar duplicados
        if (appContext.Categories.Any()) return;

        // CATEGORIES
        var categories = new List<Category>
        {
            new Category { Name = "Novela" },
            new Category { Name = "Tecnología" },
            new Category { Name = "Historia" },
            new Category { Name = "Negocios" }
        };

        appContext.Categories.AddRange(categories);
        appContext.SaveChanges();

        // AUTHORS
        var authors = new List<Author>
        {
            new Author { Name = "Gabriel García Márquez", Bio = "Escritor colombiano" },
            new Author { Name = "Yuval Noah Harari", Bio = "Historiador y escritor" },
            new Author { Name = "Robert C. Martin", Bio = "Experto en software" },
            new Author { Name = "Stephen King", Bio = "Autor de novelas de terror" }
        };

        appContext.Authors.AddRange(authors);
        appContext.SaveChanges();

        // BOOKS
        var books = new List<Book>
        {
            new Book
            {
                Title = "Cien Años de Soledad",
                Description = "Novela clásica del realismo mágico",
                Price = 350,
                ISBN = "9781234567897",
                Stock = 10,
                CategoryId = categories[0].Id,
                AuthorId = authors[0].Id
            },
            new Book
            {
                Title = "Sapiens",
                Description = "Historia de la humanidad",
                Price = 420,
                ISBN = "9781234567898",
                Stock = 15,
                CategoryId = categories[2].Id,
                AuthorId = authors[1].Id
            },
            new Book
            {
                Title = "Clean Code",
                Description = "Buenas prácticas de programación",
                Price = 500,
                ISBN = "9781234567899",
                Stock = 8,
                CategoryId = categories[1].Id,
                AuthorId = authors[2].Id
            },
            new Book
            {
                Title = "It",
                Description = "Novela de terror",
                Price = 300,
                ISBN = "9781234567800",
                Stock = 12,
                CategoryId = categories[0].Id,
                AuthorId = authors[3].Id
            }
        };

        appContext.Books.AddRange(books);
        appContext.SaveChanges();

        // USERS (Identity)
        var hasher = new PasswordHasher<ApplicationUser>();

        var user = new ApplicationUser
        {
            UserName = "admin@libros.com",
            Email = "admin@libros.com",
            Name = "Administrador"
        };

        user.PasswordHash = hasher.HashPassword(user, "Admin123*");

        if (!appContext.Users.Any())
        {
            appContext.Users.Add(user);
            appContext.SaveChanges();
        }

        // ORDER
        var order = new Order
        {
            UserId = user.Id,
            Total = 850,
            Status = OrderStatus.Pending
        };

        appContext.Orders.Add(order);
        appContext.SaveChanges();

        // ORDER DETAILS
        var orderDetails = new List<OrderDetail>
        {
            new OrderDetail
            {
                OrderId = order.Id,
                BookId = books[0].Id,
                Quantity = 1,
                Price = books[0].Price
            },
            new OrderDetail
            {
                OrderId = order.Id,
                BookId = books[2].Id,
                Quantity = 1,
                Price = books[2].Price
            }
        };

        appContext.OrderDetails.AddRange(orderDetails);
        appContext.SaveChanges();

        // recalcular total 
        order.Total = orderDetails.Sum(x => x.Price * x.Quantity);
        appContext.SaveChanges();
    }
}