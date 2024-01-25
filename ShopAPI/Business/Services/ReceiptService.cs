using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Validation;

namespace Business.Services
{
    public class ReceiptService : IReceiptService
    {
        private IUnitOfWork UnitOfWork { get; }

        private IMapper Mapper { get; }

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receiptDto is null)
                throw new MarketException($"Receipt doesn't exist");

            ReceiptDetail receiptDetailDto = receiptDto.ReceiptDetails.FirstOrDefault(x => x.ProductId == productId);

            if (receiptDetailDto is null)
            {
                Product productDto = await UnitOfWork.ProductRepository.GetByIdAsync(productId);

                if (productDto is null)
                    throw new MarketException($"Product doesn't exist");

                int customerDiscount = receiptDto.Customer.DiscountValue;
                decimal newProductUnitPrice = productDto.Price;
                decimal newProductDiscountUnitPrice = newProductUnitPrice * (100 - customerDiscount) / 100;

                ReceiptDetailModel receiptDetailModel = new()
                {
                    ReceiptId = receiptId,
                    UnitPrice = newProductUnitPrice,
                    DiscountUnitPrice = newProductDiscountUnitPrice,
                    Quantity = quantity,
                    ProductId = productId,
                };

                receiptDetailDto = Mapper.Map<ReceiptDetail>(receiptDetailModel);
                await UnitOfWork.ReceiptDetailRepository.AddAsync(receiptDetailDto);
            }
            else
            {
                receiptDetailDto.Quantity += quantity;
                UnitOfWork.ReceiptDetailRepository.Update(receiptDetailDto);
            }

            await UnitOfWork.SaveAsync();
        }

        public async Task CheckOutAsync(int receiptId)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdAsync(receiptId);

            if (receiptDto is null)
                throw new MarketException($"Receipt doesn't exist");

            receiptDto.IsCheckedOut = true;
            UnitOfWork.ReceiptRepository.Update(receiptDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receiptDto is null)
                throw new MarketException($"Receipt doesn't exist"); 

            IEnumerable<ReceiptDetailModel> receiptDetailModels = receiptDto.ReceiptDetails
                .Select(x => Mapper.Map<ReceiptDetailModel>(x));

            if (!receiptDetailModels.Any())
                throw new MarketException($"Receipt details don't exist");

            return receiptDetailModels;
        }

        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Receipt> receiptDtos = await UnitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            if (!receiptDtos.Any())
                throw new MarketException($"No receipt matched in period");

            IEnumerable<ReceiptModel> receiptModels = receiptDtos.Select(x => Mapper.Map<ReceiptModel>(x));
            receiptModels = receiptModels
                .Where(rm => rm.OperationDate >= startDate 
                            && rm.OperationDate <= endDate);
            return receiptModels;
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            ReceiptDetail receiptDetailDto = receiptDto.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

            if (receiptDetailDto is null)
                throw new MarketException($"Product doesn't exist");

            receiptDto.ReceiptDetails.Remove(receiptDetailDto);

            if (quantity >= receiptDetailDto.Quantity)
                UnitOfWork.ReceiptDetailRepository.Delete(receiptDetailDto);
            else
            {
                receiptDetailDto.Quantity -= quantity;
                receiptDto.ReceiptDetails.Add(receiptDetailDto);
            }

            UnitOfWork.ReceiptRepository.Update(receiptDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receiptDto is null)
                throw new MarketException($"Receipt doesn't exist"); 

            IEnumerable<ReceiptDetailModel> receiptDetailModels = receiptDto.ReceiptDetails
                .Select(x => Mapper.Map<ReceiptDetailModel>(x));

            if (!receiptDetailModels.Any())
                throw new MarketException($"Receipt details don't exist");

            decimal sumToPay = receiptDetailModels.Sum(x => x.DiscountUnitPrice * x.Quantity);
            return sumToPay;
        }

        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            IEnumerable<Receipt> receiptDtos = await UnitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            if (!receiptDtos.Any())
                throw new MarketException($"Receipt list is empty");

            IEnumerable<ReceiptModel> receiptModels = receiptDtos
                .Select(x => Mapper.Map<ReceiptModel>(x));
            return receiptModels;
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(id);

            if (receiptDto is null)
                throw new MarketException($"Receipt with id={id} doesn't exist"); 

            ReceiptModel receiptModel = Mapper.Map<ReceiptModel>(receiptDto);
            return receiptModel;
        }

        public async Task AddAsync(ReceiptModel model)
        {
            Receipt receiptDto = Mapper.Map<Receipt>(model);
            await UnitOfWork.ReceiptRepository.AddAsync(receiptDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            Receipt receiptDto = Mapper.Map<Receipt>(model);
            UnitOfWork.ReceiptRepository.Update(receiptDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            Receipt receiptDto = await UnitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(modelId);

            if (receiptDto is null)
                throw new MarketException($"Can't delete non-exist receipt"); 

            foreach (ReceiptDetail receiptDetailDto in receiptDto.ReceiptDetails) 
            {
                UnitOfWork.ReceiptDetailRepository.Delete(receiptDetailDto);
            }
            await UnitOfWork.ReceiptRepository.DeleteByIdAsync(modelId);
            await UnitOfWork.SaveAsync();
        }
    }
}
