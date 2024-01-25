using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private TradeMarketDbContext Context { get; set; }

        private ICustomerRepository _customerRepository;

        private IPersonRepository _personRepository;

        private IProductRepository _productRepository;

        private IProductCategoryRepository _productCategoryRepository;

        private IReceiptRepository _receiptRepository;

        private IReceiptDetailRepository _receiptDetailRepository;

        public ICustomerRepository CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                    _customerRepository = new CustomerRepository(Context);
                return _customerRepository;
            }
        }

        public IPersonRepository PersonRepository
        {
            get
            {
                if (_personRepository == null)
                    _personRepository = new PersonRepository(Context);
                return _personRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(Context);
                return _productRepository;
            }
        }

        public IProductCategoryRepository ProductCategoryRepository
        {
            get
            {
                if (_productCategoryRepository == null)
                    _productCategoryRepository = new ProductCategoryRepository(Context);
                return _productCategoryRepository;
            }
        }

        public IReceiptRepository ReceiptRepository
        {
            get
            {
                if (_receiptRepository == null)
                    _receiptRepository = new ReceiptRepository(Context);
                return _receiptRepository;
            }
        }

        public IReceiptDetailRepository ReceiptDetailRepository
        {
            get
            {
                if (_receiptDetailRepository == null)
                    _receiptDetailRepository = new ReceiptDetailRepository(Context);
                return _receiptDetailRepository;
            }
        }

        public UnitOfWork()
        {
            Context = new(new DbContextOptions<TradeMarketDbContext>());
        }

        public UnitOfWork(TradeMarketDbContext context)
        {
            Context = context;
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
