using System;
using System.ComponentModel.DataAnnotations;

namespace OrderCatalog.Model
{
    public class Order
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public string UserId { get; set; }

        // Add other properties or filters/ attributes as required.
    }
}
