using CLDV6212POE.Models;
using CLDV6212POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV6212POE.Controllers
{
    public class FilesController : Controller
    {
        public readonly AzureFileShareService _fileShareService;

        public FilesController(AzureFileShareService fileShareService)
        {
            _fileShareService = fileShareService;
        }
        public async Task <IActionResult> Index()
        {
            List<FileModel> files;
            try 
            {
                files = await _fileShareService.ListFilesAsync("uploads");
            }
            catch(Exception ex)
            {
                //log the error
                ViewBag.ErrorMessage = "Error retrieving files: " + ex.Message;
                files = new List<FileModel>();
            }
            return View(files);
        }
        [HttpPost]

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return await Index();
            }
            try
            {
                using(var stream = file.OpenReadStream())
                {
                    string directoryName = "uploads";
                    string fileName = file.FileName;
                    await _fileShareService.UploadFileAsync(directoryName, fileName, stream);
                }
                TempData["Message"] = $"File '{file.FileName}' uploaded successfully.";
            }
            catch(Exception ex)
            {
                TempData["Message"] = $"Error uploading file: {ex.Message}";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]

        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name cannot be null or empty");
            }
            try
            {
                var filestream = await _fileShareService.DownloadFileAsync("uploads", fileName);
                if(filestream == null)
                {
                    return NotFound("File not found");
                }
                return File(filestream, "application/octet-stream", fileName);
            }
            catch(Exception e)
            {
                return BadRequest($"Error downloading file: {e.Message}");
            }
        }
    }
}
