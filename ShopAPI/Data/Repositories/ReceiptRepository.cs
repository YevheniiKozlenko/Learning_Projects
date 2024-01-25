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
    public class ReceiptRepository : BaseRepository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(TradeMarketDbContext context)
        : base(context)
        {
        }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            return await Table
                .Include(r => r.Customer)
                .ThenInclude(c => c.Person)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            return await Table.Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
