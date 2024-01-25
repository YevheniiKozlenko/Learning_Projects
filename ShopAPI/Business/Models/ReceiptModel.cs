using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;

namespace Business.Models
{
    public class ReceiptModel : IModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public DateTime OperationDate { get; set; }

        public bool IsCheckedOut { get; set; }

        public ICollection<int> ReceiptDetailsIds { get; set; } = new List<int>();
    }
}
