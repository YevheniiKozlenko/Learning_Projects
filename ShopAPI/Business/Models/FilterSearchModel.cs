using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;

namespace Business.Models
{
    public class FilterSearchModel : IModel
    {
        public int? CategoryId { get; set; }

        public int? MaxPrice { get; set; }

        public int? MinPrice { get; set;}
    }
}
