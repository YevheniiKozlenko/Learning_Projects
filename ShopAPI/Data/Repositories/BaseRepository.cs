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
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        public TradeMarketDbContext Context { get; set; }
        
        public DbSet<TEntity> Table { get; set; }

        public BaseRepository(TradeMarketDbContext context)
        {
            Context = context;
            Table = Context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await Table.AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            Table.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            Table.Remove(await Table.FindAsync(id));
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await Table.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            Table.Update(entity);
        }
    }
}
