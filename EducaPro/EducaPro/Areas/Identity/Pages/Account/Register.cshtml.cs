using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EducaPro.Models;

namespace EducaPro.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public class InputModel
        {
            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un rol.")]
            public string Role { get; set; }
        }

        public async Task OnGetAsync()
        {
            string[] roles = { "Estudiante", "Profesor" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return Page();
            }

            // Verificar si el rol es Profesor y el correo tiene el dominio @upn.edu.pe
            if (Input.Role == "Profesor" && !Input.Email.EndsWith("@upn.edu.pe"))
            {
                ModelState.AddModelError("Input.Email", "Solo se pueden registrar correos con el dominio @upn.edu.pe para el rol de Profesor.");
                Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = true,
                MFASecret = GenerateMFACode() // Generar el código MFA
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(Input.Role) && await _roleManager.RoleExistsAsync(Input.Role))
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                }

                // Redirigir a la página de confirmación MFA
                TempData["MFACode"] = user.MFASecret;
                return RedirectToPage("./ConfirmMFA", new { email = user.Email });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return Page();
        }


        private string GenerateMFACode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Código de 6 dígitos
        }
    }
}
