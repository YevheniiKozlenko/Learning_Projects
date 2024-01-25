using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table(nameof(ReceiptDetail))]
    public class ReceiptDetail : BaseEntity
    {
        [ForeignKey(nameof(Receipt))]
        public int? ReceiptId { get; set; }

        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }

        public decimal DiscountUnitPrice { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public Receipt Receipt { get; set; }

        public Product Product { get; set; }
    }
}