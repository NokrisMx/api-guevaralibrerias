using ApiGuevaraLibrerias.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Data;

public static class DataSeeder
{
    public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        // ──────────────────────────────────────────────
        // ROLES
        // ──────────────────────────────────────────────
        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ──────────────────────────────────────────────
        // USUARIOS
        // ──────────────────────────────────────────────
        ApplicationUser? adminUser = await userManager.FindByEmailAsync("admin@guevaralibреrias.mx");
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                Name = "Administrador Guevara",
                UserName = "admin",
                Email = "admin@guevaralibrerias.mx",
                PhoneNumber = "8112345678",
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(adminUser, "Admin@1234!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        ApplicationUser? regularUser = await userManager.FindByEmailAsync("juan@correo.com");
        if (regularUser is null)
        {
            regularUser = new ApplicationUser
            {
                Name = "Juan Pérez",
                UserName = "juanperez",
                Email = "juan@correo.com",
                PhoneNumber = "8198765432",
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(regularUser, "User@1234!");
            await userManager.AddToRoleAsync(regularUser, "User");
        }

        // ──────────────────────────────────────────────
        // CATEGORÍAS  (6)
        // ──────────────────────────────────────────────
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new() { Name = "Fantasía" },
                new() { Name = "Terror" },
                new() { Name = "Ciencia Ficción" },
                new() { Name = "Literatura Latinoamericana" },
                new() { Name = "Historia" },
                new() { Name = "Thriller" },
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // ──────────────────────────────────────────────
        // AUTORES  (6)
        // ──────────────────────────────────────────────
        if (!context.Authors.Any())
        {
            var authors = new List<Author>
            {
                new()
                {
                    Name = "Joe Abercrombie",
                    Bio  = "Escritor británico de fantasía épica, conocido por su estilo oscuro y moralmente complejo. Creador de la saga 'El Primer Law'."
                },
                new()
                {
                    Name = "Stephen King",
                    Bio  = "Maestro del terror contemporáneo. Autor de más de 60 novelas y 200 relatos cortos, muchos adaptados al cine y la televisión."
                },
                new()
                {
                    Name = "George R. R. Martin",
                    Bio  = "Escritor y productor estadounidense, creador de la saga 'Canción de Hielo y Fuego', base de la serie 'Game of Thrones'."
                },
                new()
                {
                    Name = "Gabriel García Márquez",
                    Bio  = "Premio Nobel de Literatura 1982. Máximo exponente del realismo mágico y de la literatura latinoamericana del siglo XX."
                },
                new()
                {
                    Name = "Isaac Asimov",
                    Bio  = "Bioquímico y prolífico autor de ciencia ficción y divulgación científica. Creador de las Tres Leyes de la Robótica."
                },
                new()
                {
                    Name = "Gillian Flynn",
                    Bio  = "Escritora y guionista estadounidense, especializada en thrillers psicológicos. Autora de 'Perdida', best-seller mundial."
                },
            };
            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }

        // ──────────────────────────────────────────────
        // EDITORIALES  (6)
        // ──────────────────────────────────────────────
        if (!context.Publishers.Any())
        {
            var publishers = new List<Publisher>
            {
                new() { Name = "Alianza Editorial" },
                new() { Name = "Scribner" },
                new() { Name = "Bantam Books" },
                new() { Name = "Editorial Sudamericana" },
                new() { Name = "Gnome Press" },
                new() { Name = "Crown Publishing" },
            };
            await context.Publishers.AddRangeAsync(publishers);
            await context.SaveChangesAsync();
        }

        // ──────────────────────────────────────────────
        // LIBROS  (30)
        // ──────────────────────────────────────────────
        if (!context.Books.Any())
        {
            // Helpers para obtener IDs
            int CatId(string name) => context.Categories.First(c => c.Name == name).Id;
            int AuthId(string name) => context.Authors.First(a => a.Name == name).Id;
            int PubId(string name) => context.Publishers.First(p => p.Name == name).Id;

            var books = new List<Book>
            {
                // ── Joe Abercrombie (4) – Fantasía / Alianza Editorial ──────────────
                new()
                {
                    Title         = "La voz de las espadas",
                    Description   = "Primera entrega de la trilogía 'The First Law'. Un mundo de política, guerra y magia sin héroes absolutos.",
                    Price         = 349.00m,
                    Pages         = 531,
                    ISBN          = "9780575077881",
                    Stock         = 15,
                    YearPublished = new DateTime(2006, 5, 4),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("Joe Abercrombie"),
                    PublisherId   = PubId("Alianza Editorial"),
                },
                new()
                {
                    Title         = "Antes de que los Cuelguen",
                    Description   = "Segunda parte de 'The First Law'. La guerra avanza y los personajes se revelan en toda su complejidad moral.",
                    Price         = 349.00m,
                    Pages         = 543,
                    ISBN          = "9780575077904",
                    Stock         = 12,
                    YearPublished = new DateTime(2007, 3, 15),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("Joe Abercrombie"),
                    PublisherId   = PubId("Alianza Editorial"),
                },
                new()
                {
                    Title         = "El Último Argumento de los Reyes",
                    Description   = "Conclusión épica de la trilogía 'The First Law'. Las consecuencias de cada elección llegan con peso implacable.",
                    Price         = 349.00m,
                    Pages         = 639,
                    ISBN          = "9780575077928",
                    Stock         = 10,
                    YearPublished = new DateTime(2008, 3, 20),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("Joe Abercrombie"),
                    PublisherId   = PubId("Alianza Editorial"),
                },
                new()
                {
                    Title         = "Los Héroes",
                    Description   = "Novela independiente en el mundo de The First Law. Una batalla de tres días que define el destino de un reino.",
                    Price         = 329.00m,
                    Pages         = 527,
                    ISBN          = "9780575082793",
                    Stock         = 8,
                    YearPublished = new DateTime(2011, 3, 3),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("Joe Abercrombie"),
                    PublisherId   = PubId("Alianza Editorial"),
                },

                // ── Stephen King (4) – Terror / Scribner ──────────────────────
                new()
                {
                    Title         = "It",
                    Description   = "En Derry, Maine, un grupo de niños enfrenta a una entidad maligna que adopta la forma de sus mayores miedos.",
                    Price         = 399.00m,
                    Pages         = 1138,
                    ISBN          = "9781501156700",
                    Stock         = 20,
                    YearPublished = new DateTime(1986, 9, 15),
                    CategoryId    = CatId("Terror"),
                    AuthorId      = AuthId("Stephen King"),
                    PublisherId   = PubId("Scribner"),
                },
                new()
                {
                    Title         = "El Resplandor",
                    Description   = "Jack Torrance lleva a su familia al Hotel Overlook para pasar el invierno. El aislamiento y el hotel despertarán algo oscuro.",
                    Price         = 359.00m,
                    Pages         = 447,
                    ISBN          = "9780307743657",
                    Stock         = 18,
                    YearPublished = new DateTime(1977, 1, 28),
                    CategoryId    = CatId("Terror"),
                    AuthorId      = AuthId("Stephen King"),
                    PublisherId   = PubId("Scribner"),
                },
                new()
                {
                    Title         = "Misery",
                    Description   = "El escritor Paul Sheldon queda atrapado en la casa de su 'fan número uno' tras un accidente. Una pesadilla claustrofóbica.",
                    Price         = 319.00m,
                    Pages         = 338,
                    ISBN          = "9781982186364",
                    Stock         = 14,
                    YearPublished = new DateTime(1987, 6, 8),
                    CategoryId    = CatId("Terror"),
                    AuthorId      = AuthId("Stephen King"),
                    PublisherId   = PubId("Scribner"),
                },
                new()
                {
                    Title         = "La Torre Oscura: El Pistolero",
                    Description   = "Roland Deschain persigue al Hombre de Negro a través de un desierto interminable. Inicio de la saga magna de King.",
                    Price         = 329.00m,
                    Pages         = 244,
                    ISBN          = "9780451210845",
                    Stock         = 16,
                    YearPublished = new DateTime(1982, 6, 10),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("Stephen King"),
                    PublisherId   = PubId("Scribner"),
                },

                // ── George R. R. Martin (4) – Fantasía / Bantam Books ──────────
                new()
                {
                    Title         = "Juego de Tronos",
                    Description   = "Siete familias nobles luchan por el control del Trono de Hierro de los Siete Reinos, mientras una antigua amenaza despierta en el norte.",
                    Price         = 429.00m,
                    Pages         = 694,
                    ISBN          = "9780553103540",
                    Stock         = 25,
                    YearPublished = new DateTime(1996, 8, 1),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("George R. R. Martin"),
                    PublisherId   = PubId("Bantam Books"),
                },
                new()
                {
                    Title         = "Choque de Reyes",
                    Description   = "Tras la muerte del rey, cinco reyes reclaman el trono simultáneamente en una guerra que devastará Westeros.",
                    Price         = 429.00m,
                    Pages         = 761,
                    ISBN          = "9780553108033",
                    Stock         = 20,
                    YearPublished = new DateTime(1998, 11, 16),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("George R. R. Martin"),
                    PublisherId   = PubId("Bantam Books"),
                },
                new()
                {
                    Title         = "Tormenta de Espadas",
                    Description   = "La guerra de los Cinco Reyes llega a su punto más sangriento. Giros impredecibles y consecuencias devastadoras.",
                    Price         = 449.00m,
                    Pages         = 973,
                    ISBN          = "9780553106633",
                    Stock         = 18,
                    YearPublished = new DateTime(2000, 8, 8),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("George R. R. Martin"),
                    PublisherId   = PubId("Bantam Books"),
                },
                new()
                {
                    Title         = "Festín de Cuervos",
                    Description   = "Mientras el invierno se acerca, el reino sana sus heridas, pero nuevas conspiraciones tejen su tela en las sombras.",
                    Price         = 439.00m,
                    Pages         = 784,
                    ISBN          = "9780553801507",
                    Stock         = 15,
                    YearPublished = new DateTime(2005, 10, 17),
                    CategoryId    = CatId("Fantasía"),
                    AuthorId      = AuthId("George R. R. Martin"),
                    PublisherId   = PubId("Bantam Books"),
                },

                // ── García Márquez (3) – Literatura Latinoamericana / Sudamericana
                new()
                {
                    Title         = "Cien Años de Soledad",
                    Description   = "La saga de la familia Buendía en el pueblo ficticio de Macondo. Obra cumbre del realismo mágico y de la literatura universal.",
                    Price         = 320.00m,
                    Pages         = 471,
                    ISBN          = "9780060883287",
                    Stock         = 30,
                    YearPublished = new DateTime(1967, 5, 30),
                    CategoryId    = CatId("Literatura Latinoamericana"),
                    AuthorId      = AuthId("Gabriel García Márquez"),
                    PublisherId   = PubId("Editorial Sudamericana"),
                },
                new()
                {
                    Title         = "El Amor en los Tiempos del Cólera",
                    Description   = "Una historia de amor que espera más de cincuenta años para consumarse, ambientada en el Caribe colombiano.",
                    Price         = 295.00m,
                    Pages         = 348,
                    ISBN          = "9780307389732",
                    Stock         = 22,
                    YearPublished = new DateTime(1985, 11, 5),
                    CategoryId    = CatId("Literatura Latinoamericana"),
                    AuthorId      = AuthId("Gabriel García Márquez"),
                    PublisherId   = PubId("Editorial Sudamericana"),
                },
                new()
                {
                    Title         = "Crónica de una Muerte Anunciada",
                    Description   = "Un pueblo entero sabe que Santiago Nasar va a morir, pero nadie hace nada por impedirlo. Magistral novela corta.",
                    Price         = 249.00m,
                    Pages         = 120,
                    ISBN          = "9780307764805",
                    Stock         = 28,
                    YearPublished = new DateTime(1981, 4, 6),
                    CategoryId    = CatId("Literatura Latinoamericana"),
                    AuthorId      = AuthId("Gabriel García Márquez"),
                    PublisherId   = PubId("Editorial Sudamericana"),
                },

                // ── Isaac Asimov (5) – Ciencia Ficción / Gnome Press ──────────
                new()
                {
                    Title         = "Fundación",
                    Description   = "El matemático Hari Seldon predice la caída del Imperio Galáctico y urde un plan para preservar el conocimiento de la humanidad.",
                    Price         = 310.00m,
                    Pages         = 255,
                    ISBN          = "9780553293357",
                    Stock         = 20,
                    YearPublished = new DateTime(1951, 5, 1),
                    CategoryId    = CatId("Ciencia Ficción"),
                    AuthorId      = AuthId("Isaac Asimov"),
                    PublisherId   = PubId("Gnome Press"),
                },
                new()
                {
                    Title         = "Fundación e Imperio",
                    Description   = "La Fundación enfrenta su mayor amenaza: un mutante imprevisible conocido como el Mulo que distorsiona el Plan Seldon.",
                    Price         = 310.00m,
                    Pages         = 247,
                    ISBN          = "9780553293371",
                    Stock         = 16,
                    YearPublished = new DateTime(1952, 1, 1),
                    CategoryId    = CatId("Ciencia Ficción"),
                    AuthorId      = AuthId("Isaac Asimov"),
                    PublisherId   = PubId("Gnome Press"),
                },
                new()
                {
                    Title         = "Segunda Fundación",
                    Description   = "El Mulo busca la Segunda Fundación oculta mientras la Galaxia se debate entre el orden y el caos.",
                    Price         = 310.00m,
                    Pages         = 244,
                    ISBN          = "9780553293395",
                    Stock         = 14,
                    YearPublished = new DateTime(1953, 1, 1),
                    CategoryId    = CatId("Ciencia Ficción"),
                    AuthorId      = AuthId("Isaac Asimov"),
                    PublisherId   = PubId("Gnome Press"),
                },
                new()
                {
                    Title         = "Yo, Robot",
                    Description   = "Colección de relatos que exploran las Tres Leyes de la Robótica y sus paradojas a través de la psicóloga robótica Susan Calvin.",
                    Price         = 289.00m,
                    Pages         = 253,
                    ISBN          = "9780553294385",
                    Stock         = 25,
                    YearPublished = new DateTime(1950, 12, 2),
                    CategoryId    = CatId("Ciencia Ficción"),
                    AuthorId      = AuthId("Isaac Asimov"),
                    PublisherId   = PubId("Gnome Press"),
                },
                new()
                {
                    Title         = "El Hombre Bicentenario",
                    Description   = "Andrew Martin es un robot que pasa dos siglos buscando ser reconocido como ser humano. Una reflexión sobre la identidad y la mortalidad.",
                    Price         = 269.00m,
                    Pages         = 198,
                    ISBN          = "9780553294002",
                    Stock         = 11,
                    YearPublished = new DateTime(1976, 9, 1),
                    CategoryId    = CatId("Ciencia Ficción"),
                    AuthorId      = AuthId("Isaac Asimov"),
                    PublisherId   = PubId("Gnome Press"),
                },

                // ── Gillian Flynn (4) – Thriller / Crown Publishing ────────────
                new()
                {
                    Title         = "Perdida",
                    Description   = "La mañana de su quinto aniversario, Amy Dunne desaparece. Su esposo Nick se convierte en el principal sospechoso.",
                    Price         = 335.00m,
                    Pages         = 422,
                    ISBN          = "9780307588364",
                    Stock         = 22,
                    YearPublished = new DateTime(2012, 6, 5),
                    CategoryId    = CatId("Thriller"),
                    AuthorId      = AuthId("Gillian Flynn"),
                    PublisherId   = PubId("Crown Publishing"),
                },
                new()
                {
                    Title         = "Objetos Cortantes",
                    Description   = "Camille Preaker regresa a su pueblo natal para cubrir el asesinato de dos niñas y descubre oscuros secretos familiares.",
                    Price         = 299.00m,
                    Pages         = 254,
                    ISBN          = "9780307341556",
                    Stock         = 14,
                    YearPublished = new DateTime(2006, 7, 11),
                    CategoryId    = CatId("Thriller"),
                    AuthorId      = AuthId("Gillian Flynn"),
                    PublisherId   = PubId("Crown Publishing"),
                },
                new()
                {
                    Title         = "Lugares Oscuros",
                    Description   = "Libby Day sobrevivió a la masacre de su familia y 25 años después debe revisar el pasado para limpiar el nombre de su hermano.",
                    Price         = 315.00m,
                    Pages         = 349,
                    ISBN          = "9780307341570",
                    Stock         = 12,
                    YearPublished = new DateTime(2009, 2, 3),
                    CategoryId    = CatId("Thriller"),
                    AuthorId      = AuthId("Gillian Flynn"),
                    PublisherId   = PubId("Crown Publishing"),
                },

                // ── Extras repartidos en Historia y Literatura Latinoamericana ──
                new()
                {
                    Title         = "Sapiens: De Animales a Dioses",
                    Description   = "Yuval Noah Harari traza la historia de la humanidad desde los primeros humanos hasta la era digital con una mirada provocadora.",
                    Price         = 385.00m,
                    Pages         = 443,
                    ISBN          = "9780062316097",
                    Stock         = 30,
                    YearPublished = new DateTime(2011, 1, 1),
                    CategoryId    = CatId("Historia"),
                    AuthorId      = AuthId("Gabriel García Márquez"), // autor placeholder, reemplazar si añades autor
                    PublisherId   = PubId("Editorial Sudamericana"),
                },
                new()
                {
                    Title         = "El Otoño del Patriarca",
                    Description   = "Un dictador caribeño vive aislado en su palacio durante siglos. Una novela sobre el poder absoluto y la soledad del tirano.",
                    Price         = 275.00m,
                    Pages         = 317,
                    ISBN          = "9780060916121",
                    Stock         = 17,
                    YearPublished = new DateTime(1975, 3, 1),
                    CategoryId    = CatId("Literatura Latinoamericana"),
                    AuthorId      = AuthId("Gabriel García Márquez"),
                    PublisherId   = PubId("Editorial Sudamericana"),
                },
                new()
                {
                    Title         = "Historia de México: Del Imperio al PRI",
                    Description   = "Un recorrido accesible por los momentos decisivos de la historia mexicana desde la Independencia hasta el siglo XX.",
                    Price         = 260.00m,
                    Pages         = 390,
                    ISBN          = "9786070713461",
                    Stock         = 12,
                    YearPublished = new DateTime(2015, 9, 1),
                    CategoryId    = CatId("Historia"),
                    AuthorId      = AuthId("Gabriel García Márquez"), // placeholder
                    PublisherId   = PubId("Editorial Sudamericana"),
                },
                new()
                {
                    Title         = "La Vorágine",
                    Description   = "Clásico de la literatura colombiana. Arturo Cova huye con su amada hacia los llanos y termina atrapado en la selva amazónica.",
                    Price         = 230.00m,
                    Pages         = 316,
                    ISBN          = "9789584238252",
                    Stock         = 9,
                    YearPublished = new DateTime(1924, 11, 24),
                    CategoryId    = CatId("Literatura Latinoamericana"),
                    AuthorId      = AuthId("Gabriel García Márquez"), // placeholder
                    PublisherId   = PubId("Editorial Sudamericana"),
                },
                new()
                {
                    Title         = "Imperios del Mundo Antiguo",
                    Description   = "Análisis comparado de los grandes imperios de la antigüedad: Persa, Romano, Macedonio, Han y Maurya.",
                    Price         = 420.00m,
                    Pages         = 512,
                    ISBN          = "9788499926667",
                    Stock         = 7,
                    YearPublished = new DateTime(2018, 4, 12),
                    CategoryId    = CatId("Historia"),
                    AuthorId      = AuthId("Isaac Asimov"), // placeholder
                    PublisherId   = PubId("Gnome Press"),
                },
            };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }

        // ──────────────────────────────────────────────
        // ÓRDENES  (5 por usuario = 10 total)
        // ──────────────────────────────────────────────
        if (!context.Orders.Any())
        {
            var bookIds = context.Books.Select(b => new { b.Id, b.Price }).ToList();
            var random = new Random(42);

            var users = new[] { adminUser, regularUser };

            foreach (var user in users)
            {
                for (int o = 1; o <= 5; o++)
                {
                    // Elegir entre 2 y 4 libros distintos al azar por orden
                    var selectedBooks = bookIds
                        .OrderBy(_ => random.Next())
                        .Take(random.Next(2, 5))
                        .ToList();

                    var details = selectedBooks.Select(b => new OrderDetail
                    {
                        BookId = b.Id,
                        Quantity = random.Next(1, 4),
                        Price = b.Price,
                    }).ToList();

                    var total = details.Sum(d => d.Price * d.Quantity);

                    var statuses = new[] { OrderStatus.Pending, OrderStatus.Paid, OrderStatus.Cancelled };

                    var order = new Order
                    {
                        UserId = user!.Id,
                        Total = total,
                        Status = statuses[random.Next(statuses.Length)],
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 180)),
                        OrderDetails = details,
                    };

                    await context.Orders.AddAsync(order);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}