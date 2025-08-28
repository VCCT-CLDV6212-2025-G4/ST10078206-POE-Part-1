using Microsoft.AspNetCore.Mvc;
using CLDV6212POE.Services;
using CLDV6212POE.Models;

namespace CLDV6212POE.Controllers
{
    public class UserController : Controller
    {
        private readonly TableStorageService _tableStorageService;

        public UserController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _tableStorageService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(Users user)
        {
            if (ModelState.IsValid)
            {
                user.PartitionKey = "UserPartition";
                user.RowKey = Guid.NewGuid().ToString();

                await _tableStorageService.AddUserAsync(user);
                return RedirectToAction("Index");
            }

            
            return View(user);
        }

        
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteUserAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }
    }
}

