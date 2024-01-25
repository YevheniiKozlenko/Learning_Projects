using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService service)
        {
            _statisticService = service;
        }

        // GET : api/statistics/popularProducts?productCount=2
        [HttpGet("popularProducts")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetPopularProducts([FromQuery] int productCount)
        {
            try
            {
                IEnumerable<ProductModel> popularProducts = await _statisticService.GetMostPopularProductsAsync(productCount);
                return Ok(popularProducts);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET : api/statistics/customer/1/2
        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetCustomerPopularProducts(int id, int productCount)
        {
            try
            {
                IEnumerable<ProductModel> customerPopularProducts = 
                    await _statisticService.GetCustomersMostPopularProductsAsync(productCount, id);
                return Ok(customerPopularProducts);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET : api/statistics/activity/{customerCount}?startDate=2020-7-21&endDate=2020-7-22
        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostActiveCustomers(int customerCount, 
            [FromQuery] 
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                IEnumerable<CustomerActivityModel> activityModels =
                    await _statisticService.GetMostValuableCustomersAsync(customerCount, startDate, endDate);
                return Ok(activityModels);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET : api/income/{categoryId}?startDate=2020-7-21&endDate=2020-7-22
        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetIncomeByCategoryByPeriod(int categoryId, 
            [FromQuery] 
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                decimal income = await _statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate);
                return Ok(income);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
