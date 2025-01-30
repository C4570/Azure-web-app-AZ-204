using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AZ_204_SrC4.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string ErrorMessage { get; private set; } = "";

        public IActionResult OnPost()
        {
            if (Username == "admin" && Password == "123")
            {
                // Redirigimos a la página de administración
                return RedirectToPage("AdminDashboard");
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }
        }
    }
}
