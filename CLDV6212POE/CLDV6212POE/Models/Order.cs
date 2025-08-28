using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace CLDV6212POE.Models
{
    public class Order : ITableEntity
    {
        [Key]
        public string Order_Id { get; set; } = Guid.NewGuid().ToString();
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        public string User_Id { get; set; } = string.Empty;  // <-- Changed to string

        [Required(ErrorMessage = "Please select a product.")]
        public int Product_Id { get; set; }

        [Required(ErrorMessage = "Please enter the date.")]
        public DateTime Order_Date { get; set; }

        [Required(ErrorMessage = "Please enter the location.")]
        public string? Order_Location { get; set; }
    }
}



