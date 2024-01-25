﻿using Data.Data;
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
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(TradeMarketDbContext context)
        : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            return await Table.Include(c => c.Person)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdWithDetailsAsync(int id)
        {
            return await Table.Include(c => c.Person)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.ReceiptDetails)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
