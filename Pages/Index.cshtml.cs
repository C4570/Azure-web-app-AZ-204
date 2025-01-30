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
            Output = "";
        }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        public string Output { get; private set; }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (UploadedFile != null)
            {
                // Validar extensión en servidor
                var allowedExtensions = new List<string> { ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(UploadedFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    Output = "Tipo de archivo no permitido. Solo PDF, DOC, DOCX, PNG, JPG.";
                    return Page();
                }

                // Carpeta "CVs"
                var cvsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CVs");
                Directory.CreateDirectory(cvsFolder);

                // Guardamos el archivo
                var filePath = Path.Combine(cvsFolder, UploadedFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                // (Opcional) Ejecutar script de Python
                Output = RunPythonScript(filePath);
            }
            return Page();
        }


        private string RunPythonScript(string filePath)
        {
            string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "procesar.py");
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = psi };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return string.IsNullOrEmpty(errors) ? output : $"Error ejecutando Python: {errors}";
        }
    }
}
