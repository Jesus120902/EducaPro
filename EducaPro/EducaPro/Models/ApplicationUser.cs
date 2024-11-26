using Microsoft.AspNetCore.Identity;

namespace EducaPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? MFASecret { get; set; } // Nota: El signo de interrogación (?) permite valores nulos.
    }
}
