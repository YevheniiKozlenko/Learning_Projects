using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Validation;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private IUnitOfWork UnitOfWork { get; }

        private IMapper Mapper { get; }

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(
            int productCount, 
            int customerId)
        {
            IEnumerable<Receipt> receiptDtos = await UnitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            if (!receiptDtos.Any())
                throw new MarketException($"Receipts list is empty");

            IEnumerable<ProductModel> customerMostPopularProducts = receiptDtos
                .Where(r => r.CustomerId == customerId)
                .SelectMany(r => r.ReceiptDetails)
                .GroupBy(rd => rd.Product)
                .OrderByDescending(g => g.Sum(x => x.Quantity))
                .Take(productCount)
                .Select(x => Mapper.Map<ProductModel>(x.Key));

            if (!customerMostPopularProducts.Any())
                throw new MarketException($"Customer's purchases list is empty");

            return customerMostPopularProducts;
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(
            int categoryId, 
            DateTime startDate, 
            DateTime endDate)
        {
            IEnumerable<Receipt> receiptDtos = await UnitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            if (!receiptDtos.Any())
                throw new MarketException($"Receipts list is empty");

            decimal incomeOfCategoryDuringPeriod = receiptDtos
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .SelectMany(r => r.ReceiptDetails)
                .Where(rd => rd.Product.ProductCategoryId == categoryId)
                .Sum(rd => rd.DiscountUnitPrice * rd.Quantity);

            return incomeOfCategoryDuringPeriod;
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            IEnumerable<ReceiptDetail> receiptDetailDtos = await UnitOfWork.ReceiptDetailRepository.GetAllWithDetailsAsync();

            if (!receiptDetailDtos.Any())
                throw new MarketException($"Receipts of products non-exist");

            IEnumerable<ProductModel> productModels = receiptDetailDtos
                .GroupBy(rd => rd.Product)
                .OrderByDescending(g => g.Sum(rd => rd.Quantity))
                .Take(productCount)
                .Select(x => Mapper.Map<ProductModel>(x.Key));

            if (!productModels.Any())
                throw new MarketException($"Receipts were empty");
            
            return productModels;
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(
            int customerCount, 
            DateTime startDate, 
            DateTime endDate)
        {
            IEnumerable<Receipt> receiptDtos = await UnitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            if (!receiptDtos.Any())
                throw new MarketException($"Receipts list is empty");

            IEnumerable<CustomerActivityModel> customerActivityModels = receiptDtos
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .GroupBy(r => r.Customer)
                .OrderByDescending(g => 
                    g.Sum(x => x.ReceiptDetails
                        .Sum(y => y.DiscountUnitPrice * y.Quantity)))
                .Take(customerCount)
                .Select(g => new CustomerActivityModel
                {
                    CustomerId = g.Key.Id,
                    CustomerName = $"{ g.Key.Person.Name } { g.Key.Person.Surname }",
                    ReceiptSum = g.Sum(x=> x.ReceiptDetails
                        .Sum(y => y.DiscountUnitPrice * y.Quantity))
                });

            if (!customerActivityModels.Any())
                throw new MarketException($"Clients list is empty");

            return customerActivityModels;
        }
    }
}
