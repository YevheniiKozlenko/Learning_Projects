using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(TradeMarketDbContext context)
        : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await Table.Include(p => p.ReceiptDetails)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return await Table.Include(p => p.ReceiptDetails)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
