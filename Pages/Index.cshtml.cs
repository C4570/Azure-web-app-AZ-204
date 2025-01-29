using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AZ_204_SrC4.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Output = "";  // Inicializa la variable Output para evitar errores
        }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; } // Propiedad para recibir el archivo

        public string Output { get; private set; }  // Resultado del procesamiento

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (UploadedFile != null)
            {
                // Crear la carpeta 'wwwroot/uploads' si no existe
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Guardar el archivo subido
                var filePath = Path.Combine(uploadsFolder, UploadedFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                // Ejecutar el script de Python y obtener la salida
                Output = RunPythonScript(filePath);
            }

            return Page();
        }

        private string RunPythonScript(string filePath)
        {
            string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "procesar.py"); // Ruta del script Python
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "python";  // Python debe estar en la variable de entorno
            psi.Arguments = $"\"{scriptPath}\" \"{filePath}\""; // Pasar el archivo al script de Python
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return string.IsNullOrEmpty(errors) ? output : $"Error ejecutando Python: {errors}";
        }
    }
}
