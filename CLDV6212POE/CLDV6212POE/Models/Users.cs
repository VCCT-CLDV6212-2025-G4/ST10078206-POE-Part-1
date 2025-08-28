using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CLDV6212POE.Models
{
    public class Users : ITableEntity
    {
        [Key]
        public string User_Id { get; set; } 
        public string? FirstName { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }

        // ITableEntity implementation
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
