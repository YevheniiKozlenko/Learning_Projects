using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;

namespace Business.Models
{
    public class ProductCategoryModel : IModel
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public ICollection<int> ProductIds { get; set; } = new List<int>();
    }
}
