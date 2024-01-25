using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService service)
        {
            _customerService = service;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            try
            {
                IEnumerable<CustomerModel> customers = await _customerService.GetAllAsync();
                return Ok(customers);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }

        }

        //GET: api/customers/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetById(int id)
        {
            try
            {
                var needCustomer = await _customerService.GetByIdAsync(id);
                return Ok(needCustomer);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }

        }
        
        //GET: api/customers/products/1
        [HttpGet("products/{id}")]
        public async Task<ActionResult<CustomerModel>> GetByProductId(int id)
        {
            try
            {
                IEnumerable<CustomerModel> needCustomer = await _customerService.GetCustomersByProductIdAsync(id);
                return Ok(needCustomer);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }

        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CustomerModel value)
        {
            try
            {
                await _customerService.AddAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/customers/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CustomerModel value)
        {
            try
            {
                await _customerService.UpdateAsync(value);
                return Ok(id);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/customers/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
                return Ok(id);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
