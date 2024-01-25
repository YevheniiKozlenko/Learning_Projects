using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productsService;

        public ProductsController(IProductService service)
        {
            _productsService = service;
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductsById(int id)
        {
            try
            {
                ProductModel needModel = await _productsService.GetByIdAsync(id);
                return Ok(needModel);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/products?categoryId=1&minPrice=20&maxPrice=50
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get([FromQuery] 
            int? categoryId,
            int? minPrice,
            int? maxPrice)
        {
            try
            {
                IEnumerable<ProductModel> needProductModels;
                if (categoryId is not null ||
                    minPrice is not null ||
                    maxPrice is not null)
                {
                    FilterSearchModel filterModel = new()
                    {
                        CategoryId = categoryId,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice
                    };

                    needProductModels = await _productsService.GetByFilterAsync(filterModel);
                }
                else
                    needProductModels = await _productsService.GetAllAsync();

                return Ok(needProductModels);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult> PostProduct([FromBody] ProductModel value)
        {
            try
            {
                await _productsService.AddAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, [FromBody] ProductModel value)
        {
            try
            {
                await _productsService.UpdateAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productsService.DeleteAsync(id);
                return Ok(id);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/products/categories
        [HttpGet("categories")]
        public async Task<ActionResult<ProductCategoryModel>> GetCategories()
        {
            try
            {
                var categoriesModels = await _productsService.GetAllProductCategoriesAsync();
                return Ok(categoriesModels);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/products/categories
        [HttpPost("categories")]
        public async Task<ActionResult> PostCategory([FromBody] ProductCategoryModel value)
        {
            try
            {
                await _productsService.AddCategoryAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/products/categories/1
        [HttpPut("categories/{id}")]
        public async Task<ActionResult> PutCategory(int id, [FromBody] ProductCategoryModel value)
        {
            try
            {
                await _productsService.UpdateCategoryAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/products/categories/1
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                await _productsService.RemoveCategoryAsync(id);
                return Ok(id);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
