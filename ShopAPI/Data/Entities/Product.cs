using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table(nameof(Product))]
    public class Product : BaseEntity
    {
        [ForeignKey(nameof(ProductCategory))]
        public int? ProductCategoryId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public ProductCategory Category { get; set; }

        public ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();
    }
}