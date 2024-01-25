using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private IUnitOfWork UnitOfWork { get; }

        private IMapper Mapper { get; }

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            Validate(categoryModel);
            ProductCategory productCategoryDto = Mapper.Map<ProductCategory>(categoryModel);
            await UnitOfWork.ProductCategoryRepository.AddAsync(productCategoryDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            IEnumerable<ProductCategory> productCategoryDtos = await UnitOfWork.ProductCategoryRepository.GetAllAsync();

            if (!productCategoryDtos.Any())
                throw new MarketException($"Product list is empty");

            IEnumerable<ProductCategoryModel> productCategoryModels = productCategoryDtos
                .Select(x => Mapper.Map<ProductCategoryModel>(x));

            return productCategoryModels;
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            if (filterSearch is null)
                throw new MarketException("Filter is empty");

            IEnumerable<Product> productDtos = await UnitOfWork.ProductRepository.GetAllWithDetailsAsync();

            if (!productDtos.Any())
                throw new MarketException($"Product matched filter is not found");

            var needProductModels = productDtos
                .Where(x =>
                {
                    bool isNeedCategoryId = x.ProductCategoryId == 
                                            (filterSearch?.CategoryId ?? x.ProductCategoryId);

                    bool isPriceUnderMax = x.Price <=
                                        (filterSearch?.MaxPrice ?? x.Price);

                    bool isPriceAboveMin = x.Price >=
                                           (filterSearch?.MinPrice ?? x.Price);

                    return isNeedCategoryId && isPriceUnderMax && isPriceAboveMin;
                })
                .Select(x => Mapper.Map<ProductModel>(x));

            return needProductModels;
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            await UnitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await UnitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            Validate(categoryModel);
            ProductCategory productCategoryDto = Mapper.Map<ProductCategory>(categoryModel);
            UnitOfWork.ProductCategoryRepository.Update(productCategoryDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            IEnumerable<Product> productDtos = await UnitOfWork.ProductRepository.GetAllWithDetailsAsync();

            if (!productDtos.Any())
                throw new MarketException($"Product list is empty");

            IEnumerable<ProductModel> productModels = productDtos
                .Select(x => Mapper.Map<ProductModel>(x));
            return productModels;
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            Product productDto = await UnitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);

            if (productDto is null)
                throw new MarketException($"Product with id={id} not found");

            ProductModel productModel = Mapper.Map<ProductModel>(productDto);
            return productModel;
        }

        public async Task AddAsync(ProductModel model)
        {
            Validate(model);
            Product productDto = Mapper.Map<Product>(model);
            await UnitOfWork.ProductRepository.AddAsync(productDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ProductModel model)
        {
            Validate(model);
            Product productDto = Mapper.Map<Product>(model);
            UnitOfWork.ProductRepository.Update(productDto);
            await UnitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await UnitOfWork.ProductRepository.DeleteByIdAsync(modelId);
            await UnitOfWork.SaveAsync();
        }

        private static void Validate<TModel>(TModel model) where TModel : IModel
        {
            if (model is null)
                throw new MarketException("Given model is null");

            if (model is ProductModel productModel)
            {
                if (String.IsNullOrEmpty(productModel.ProductName))
                    throw new MarketException($"Given product model name is null or empty");

                if (productModel.Price <= 0)
                    throw new MarketException($"Price is negative");
            }

            else if (model is ProductCategoryModel productCategoryModel)
            {
                if (String.IsNullOrEmpty(productCategoryModel.CategoryName))
                    throw new MarketException($"Given product category model name is null or empty");
            }

            else
                throw new ArgumentException($"Invalid model used", nameof(model));
        }
    }
}
