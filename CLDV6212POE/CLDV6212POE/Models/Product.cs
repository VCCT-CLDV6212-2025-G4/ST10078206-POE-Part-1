using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CLDV6212POE.Models
{
    public class Product : ITableEntity
    {
        [Key]
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? ImageUrl { get; set; }

        // ITableEntity implementation
        public string PartitionKey { get; set; } = "ProductPartition";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}

