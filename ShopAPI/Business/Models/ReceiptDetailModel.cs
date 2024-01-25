using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;

namespace Business.Models
{
    public class ReceiptDetailModel : IModel
    {
        public int Id { get; set; }

        public int ReceiptId { get; set; }

        public int ProductId { get; set; }

        public decimal DiscountUnitPrice { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
