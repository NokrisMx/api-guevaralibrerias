using ApiGuevaraLibrerias.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext db)
    {
        SeedRoles(db);
        SeedUsers(db);
        SeedCategories(db);
        SeedAuthors(db);
        SeedBooks(db);
        SeedOrders(db);
    }

    private static void SeedRoles(ApplicationDbContext db)
    {
        if (db.Roles.Any()) return;

        db.Roles.AddRange(
            new IdentityRole
            {
                Id = "role-admin",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Id = "role-user",
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );
        db.SaveChanges();
    }

    private static void SeedUsers(ApplicationDbContext db)
    {
        if (db.Users.Any()) return;

        var hasher = new PasswordHasher<ApplicationUser>();

        var admin = new ApplicationUser
        {
            Id = "user-admin",
            Name = "Aldo Guevara",
            UserName = "aldoguevara",
            NormalizedUserName = "ALDOGUEVARA",
            Email = "admin@guevaralibrerias.com",
            NormalizedEmail = "ADMIN@GUEVARALIBRERIAS.COM",
            EmailConfirmed = true,
            PhoneNumber = "8112345678",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        var user = new ApplicationUser
        {
            Id = "user-client",
            Name = "Juan Pérez",
            UserName = "juanperez",
            NormalizedUserName = "JUANPEREZ",
            Email = "juan@gmail.com",
            NormalizedEmail = "JUAN@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "8119876543",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        user.PasswordHash = hasher.HashPassword(user, "User123!");

        db.Users.AddRange(admin, user);
        db.SaveChanges();

        // Asignar roles
        db.UserRoles.AddRange(
            new IdentityUserRole<string> { UserId = "user-admin", RoleId = "role-admin" },
            new IdentityUserRole<string> { UserId = "user-client", RoleId = "role-user" }
        );
        db.SaveChanges();
    }

    private static void SeedCategories(ApplicationDbContext db)
    {
        if (db.Categories.Any()) return;

        db.Categories.AddRange(
            new Category { Name = "Ficción", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Ciencia", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Historia", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Fantasía", CreatedAt = DateTime.UtcNow },
            new Category { Name = "Tecnología", CreatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
    }

    private static void SeedAuthors(ApplicationDbContext db)
    {
        if (db.Authors.Any()) return;

        db.Authors.AddRange(
            new Author
            {
                Name = "Gabriel García Márquez",
                Bio = "Escritor colombiano, premio Nobel de Literatura 1982, conocido por el realismo mágico.",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Name = "George Orwell",
                Bio = "Escritor británico conocido por sus obras de crítica política y social.",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Name = "J.R.R. Tolkien",
                Bio = "Escritor y filólogo británico, creador de la Tierra Media.",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Name = "Stephen Hawking",
                Bio = "Físico teórico británico, conocido por sus trabajos sobre agujeros negros y cosmología.",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Name = "Yuval Noah Harari",
                Bio = "Historiador y escritor israelí, autor de Sapiens y Homo Deus.",
                CreatedAt = DateTime.UtcNow
            }
        );
        db.SaveChanges();
    }

    private static void SeedBooks(ApplicationDbContext db)
    {
        if (db.Books.Any()) return;

        var ficcion = db.Categories.First(c => c.Name == "Ficción");
        var ciencia = db.Categories.First(c => c.Name == "Ciencia");
        var historia = db.Categories.First(c => c.Name == "Historia");
        var fantasia = db.Categories.First(c => c.Name == "Fantasía");
        var tecnologia = db.Categories.First(c => c.Name == "Tecnología");

        var garcia = db.Authors.First(a => a.Name == "Gabriel García Márquez");
        var orwell = db.Authors.First(a => a.Name == "George Orwell");
        var tolkien = db.Authors.First(a => a.Name == "J.R.R. Tolkien");
        var hawking = db.Authors.First(a => a.Name == "Stephen Hawking");
        var harari = db.Authors.First(a => a.Name == "Yuval Noah Harari");

        db.Books.AddRange(
            new Book
            {
                Title = "Cien años de soledad",
                Description = "La historia de la familia Buendía a lo largo de siete generaciones en el pueblo de Macondo.",
                Price = 299.99m,
                ISBN = "9780060883287",
                Stock = 15,
                ImgUrl = "https://placehold.co/300x300",
                CategoryId = ficcion.Id,
                AuthorId = garcia.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Book
            {
                Title = "1984",
                Description = "Una novela distópica sobre un régimen totalitario que controla cada aspecto de la vida.",
                Price = 199.99m,
                ISBN = "9780451524935",
                Stock = 20,
                ImgUrl = "https://placehold.co/300x300",
                CategoryId = ficcion.Id,
                AuthorId = orwell.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Book
            {
                Title = "El señor de los anillos",
                Description = "La épica historia de la Tierra Media y la lucha contra el señor oscuro Sauron.",
                Price = 499.99m,
                ISBN = "9780618640157",
                Stock = 10,
                ImgUrl = "https://placehold.co/300x300",
                CategoryId = fantasia.Id,
                AuthorId = tolkien.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Book
            {
                Title = "Breve historia del tiempo",
                Description = "Una introducción accesible a la cosmología y los grandes misterios del universo.",
                Price = 249.99m,
                ISBN = "9780553380163",
                Stock = 12,
                ImgUrl = "https://placehold.co/300x300",
                CategoryId = ciencia.Id,
                AuthorId = hawking.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Book
            {
                Title = "Sapiens",
                Description = "Un recorrido por la historia de la humanidad desde el homo sapiens hasta la actualidad.",
                Price = 349.99m,
                ISBN = "9780062316097",
                Stock = 18,
                ImgUrl = "https://placehold.co/300x300",
                CategoryId = historia.Id,
                AuthorId = harari.Id,
                CreatedAt = DateTime.UtcNow
            }
        );
        db.SaveChanges();
    }

    private static void SeedOrders(ApplicationDbContext db)
    {
        if (db.Orders.Any()) return;

        var book1 = db.Books.First(b => b.ISBN == "9780060883287");
        var book2 = db.Books.First(b => b.ISBN == "9780451524935");
        var book3 = db.Books.First(b => b.ISBN == "9780062316097");

        var order = new Order
        {
            UserId = "user-client",
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Paid,
            Total = book1.Price + (book2.Price * 2),
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    BookId = book1.Id,
                    Quantity = 1,
                    Price = book1.Price
                },
                new OrderDetail
                {
                    BookId = book2.Id,
                    Quantity = 2,
                    Price = book2.Price
                }
            }
        };

        var order2 = new Order
        {
            UserId = "user-client",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Status = OrderStatus.Pending,
            Total = book3.Price,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    BookId = book3.Id,
                    Quantity = 1,
                    Price = book3.Price
                }
            }
        };

        db.Orders.AddRange(order, order2);
        db.SaveChanges();
    }
}