using CLDV6212POE.Models;
using CLDV6212POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV6212POE.Controllers
{
    public class ProductController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableStorageService _tableStorageService;

        public ProductController(BlobService blobService, TableStorageService tableStorageService)
        {
            _blobService = blobService;
            _tableStorageService = tableStorageService;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Product/AddProduct
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: Product/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product, IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var imageUrl = await _blobService.UploadsAsync(stream, file.FileName);
                    product.ImageUrl = imageUrl;
                }

                // Ensure PartitionKey and RowKey
                if (string.IsNullOrEmpty(product.PartitionKey))
                    product.PartitionKey = "ProductPartition";

                if (string.IsNullOrEmpty(product.RowKey))
                    product.RowKey = Guid.NewGuid().ToString();

                await _tableStorageService.addProductAsync(product);

                TempData["Message"] = "Product added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error adding product: " + ex.Message);
                return View(product);
            }
        }

        // POST: Product/DeleteProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey, string ImageUrl)
        {
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                await _blobService.DeleteBlobAsync(ImageUrl);
            }

            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);
            TempData["Message"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}

