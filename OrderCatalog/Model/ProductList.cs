using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Model
{
    [Table("ProductList")]
    public class ProductList
    {
        [Column("ProductId")]
        public int Id { get; set; }
    }
}
