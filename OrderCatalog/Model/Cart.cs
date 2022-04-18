using System;
using System.ComponentModel.DataAnnotations;

namespace OrderCatalog.Model
{
    public class Cart
    {
        [Required]
        [Key]
        public int CartID { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Please enter Quantity between 1 and 1000")]
        public int Quantity { get; set; }
        [Required]
        
        public string UserId { get; set; } 

        // Add other properties or filters/ attributes as required.
    }
}
