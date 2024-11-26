using EducaPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace EducaPro.Areas.Identity.Pages.Account
{
    public class ConfirmMFAModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmMFAModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string MFACodeInput { get; set; } // Código ingresado por el usuario

        [BindProperty]
        public string Email { get; set; }

        public string GeneratedMFACode { get; private set; }

        public void OnGet(string email)
        {
            Email = email;

            if (TempData["MFACode"] == null)
            {
                Random random = new Random();
                GeneratedMFACode = random.Next(100000, 999999).ToString();
                TempData["MFACode"] = GeneratedMFACode;
                TempData.Keep("MFACode");
            }
            else
            {
                GeneratedMFACode = TempData["MFACode"].ToString();
                TempData.Keep("MFACode");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(MFACodeInput))
            {
                ModelState.AddModelError(string.Empty, "Debe ingresar un código.");
                return Page();
            }

            var correctMFACode = TempData["MFACode"]?.ToString();
            TempData.Keep("MFACode");

            if (MFACodeInput == correctMFACode)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    user.MFASecret = null;
                    await _userManager.UpdateAsync(user);
                }

                TempData["SuccessMessage"] = "Verificación completada con éxito.";
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "El código ingresado no es correcto.");
            return Page();
        }
    }
}
