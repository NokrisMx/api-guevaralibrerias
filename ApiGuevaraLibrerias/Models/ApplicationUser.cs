using System;
using Microsoft.AspNetCore.Identity;

namespace ApiGuevaraLibrerias.Models;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}
