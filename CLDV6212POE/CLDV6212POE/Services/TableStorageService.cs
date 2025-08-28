using Azure;
using Azure.Data.Tables;
using CLDV6212POE.Models;

namespace CLDV6212POE.Services
{
    public class TableStorageService
    {
        public readonly TableClient _userTableClient; // for user table
        public readonly TableClient _productTableClient; // for product table
        public readonly TableClient _orderTableClient; // for order table

        public TableStorageService(string connectionString)
        {
            _userTableClient = new TableClient(connectionString, "User");
            _productTableClient = new TableClient(connectionString, "Product");
            _orderTableClient = new TableClient(connectionString, "orders");
        }

        public async Task<List<Users>> GetAllUsersAsync() // was suppose to be User but it gives error
        {
            var users = new List<Users>(); // was suppose to be User but it gives error

            await foreach (var user in _userTableClient.QueryAsync<Users>())
            {
                users.Add(user);
            }

            return users;
        }

        public async Task AddUserAsync(Users user) // was suppose to be User but it gives error
        {
            if (string.IsNullOrEmpty(user.PartitionKey) || string.IsNullOrEmpty(user.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            try
            {
                await _userTableClient.AddEntityAsync(user);
            }

            catch (RequestFailedException ex) 
            {
                throw new InvalidOperationException("Error adding entity to Table storage", ex);
            }
        }

        public async Task DeleteUserAsync(string partitionKey, string rowKey)
        {
            await _userTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        // Product methods
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _productTableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task addProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }
            try
            {
                await _productTableClient.AddEntityAsync(product);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding entity to Table storage", ex);
            }
        }
        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task AddOrderAsync(Order order)
        { 
            if (string.IsNullOrEmpty(order.PartitionKey) || string.IsNullOrEmpty(order.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            try
            {
                await _orderTableClient.AddEntityAsync(order);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding order to Table storage", ex);
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            await foreach (var order in _orderTableClient.QueryAsync<Order>())
            {
                orders.Add(order);
            }
            return orders;
        }
    }
}
