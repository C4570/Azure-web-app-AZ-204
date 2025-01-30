using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AZ_204_SrC4.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public List<string> CVFiles { get; set; } = new();

        public void OnGet()
        {
            // Ruta local de la carpeta "CVs"
            string cvsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CVs");

            // Crea la carpeta si no existe
            if (!Directory.Exists(cvsFolder))
                Directory.CreateDirectory(cvsFolder);

            // Obtiene todos los archivos (cualquier extensión) en /wwwroot/CVs
            var files = Directory.GetFiles(cvsFolder, "*.*", SearchOption.TopDirectoryOnly);

            // Se guarda únicamente el nombre del archivo (sin la ruta completa)
            CVFiles = files.Select(Path.GetFileName).ToList();
        }
    }
}
