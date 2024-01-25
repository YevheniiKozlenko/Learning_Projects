using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService service)
        {
            _receiptService = service;
        }

        // GET: api/receipts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> Get()
        {
            try
            {
                IEnumerable<ReceiptModel> receipts = await _receiptService.GetAllAsync();
                return Ok(receipts);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/receipts/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            try
            {
                ReceiptModel receipt = await _receiptService.GetByIdAsync(id);
                return Ok(receipt);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/receipts/1/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetDetailsById(int id)
        {
            try
            {
                IEnumerable<ReceiptDetailModel> receiptDetails = await _receiptService.GetReceiptDetailsAsync(id);
                return Ok(receiptDetails);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/receipts/1/sum
        [HttpGet("{id}/sum")]
        public async Task<ActionResult<decimal>> GetSumById(int id)
        {
            try
            {
                decimal receiptSum = await _receiptService.ToPayAsync(id);
                return Ok(receiptSum);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET api/receipts/period?startDate=2021-12-1&endDate=2020-12-31
        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetByPeriod([FromQuery]
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                IEnumerable<ReceiptModel> receiptInPeriod = await _receiptService.GetReceiptsByPeriodAsync(startDate, endDate);
                return Ok(receiptInPeriod);
            }
            catch (MarketException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST api/receipts
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ReceiptModel value)
        {
            try
            {
                await _receiptService.AddAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/receipts/1
        [HttpPut("{id}")]
        public async Task<ActionResult> PutReceipt(int id, [FromBody] ReceiptModel value)
        {
            try
            {
                await _receiptService.UpdateAsync(value);
                return Ok(value);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/receipts/1/products/add/1/3
        [HttpPut("{receiptId}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult<string>> AddProductsToReceipt(int receiptId, int productId, int quantity)
        {
            try
            {
                await _receiptService.AddProductAsync(productId, receiptId, quantity);
                return Ok("(receiptId, productId, quantity)");
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/receipts/1/products/remove/1/3
        [HttpPut("{receiptId}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProductsToReceipt(int receiptId, int productId, int quantity)
        {
            try
            {
                await _receiptService.RemoveProductAsync(productId, receiptId, quantity);
                return Ok((receiptId, productId, quantity));
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/receipts/1/checkout
        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> CheckoutReceipt(int id)
        {
            try
            {
                await _receiptService.CheckOutAsync(id);
                return Ok(id);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/receipts/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _receiptService.DeleteAsync(id);
                return Ok(id);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
