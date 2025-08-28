using CLDV6212POE.Models;
using CLDV6212POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV6212POE.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;

        public OrdersController(TableStorageService tableStorageService, QueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var users = await _tableStorageService.GetAllUsersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            if (users == null || users.Count == 0)
            {
                ModelState.AddModelError("", "No users found. Please add users first.");
                return View();
            }
            if (products == null || products.Count == 0)
            {
                ModelState.AddModelError("", "No products found. Please add products first.");
                return View();
            }

            ViewData["Users"] = users;
            ViewData["Products"] = products;
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Set UTC date, PartitionKey, and RowKey
                order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);
                order.PartitionKey = "OrderPartition";
                order.RowKey = Guid.NewGuid().ToString();

                // Save order to Azure Table
                await _tableStorageService.AddOrderAsync(order);

                // Send message to queue
                string message = $"New order created: Order ID {order.Order_Id}, " +
                                 $"User ID {order.User_Id}, " +
                                 $"Product ID {order.Product_Id}, " +
                                 $"Date {order.Order_Date}, " +
                                 $"Location {order.Order_Location}";

                await _queueService.SendMessageAsync(message);

                // Redirect to Index after successful submission
                return RedirectToAction("Index");
            }

            // Reload users/products if model state is invalid
            ViewData["Users"] = await _tableStorageService.GetAllUsersAsync();
            ViewData["Products"] = await _tableStorageService.GetAllProductsAsync();
            return View(order);
        }
    }
}


