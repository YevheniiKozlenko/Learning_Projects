using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;

namespace Business.Models
{
    public class CustomerActivityModel : IModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public decimal ReceiptSum { get; set; }
    }
}
