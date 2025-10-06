using Azure;
using Azure.Data.Tables;

namespace CLDV6112Functions.Models
{
    public class Users : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string User_Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
