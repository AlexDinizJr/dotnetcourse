using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using api.DTOs.Stock;
using api.Repositories;
using api.Interfaces;
using api.Models;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStockRepository _stockRepo;

        public StockController(IMapper mapper, IStockRepository stockRepo)
        {
            _mapper = mapper;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepo.GetAllAsync();
            var stockDTOs = _mapper.Map<List<StockDTO>>(stocks);
            return Ok(stockDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<StockDTO>(stock));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDTO stock)
        {
            var stockModel = _mapper.Map<Stock>(stock);
            await _stockRepo.CreateAsync(stockModel);
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, _mapper.Map<StockDTO>(stockModel));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDTO stock)
        {
            var existingStock = await _stockRepo.GetByIdAsync(id);

            if (existingStock == null)
            {
                return NotFound();
            }

            _mapper.Map(stock, existingStock);
            await _stockRepo.UpdateAsync(id, stock);
            return Ok(_mapper.Map<StockDTO>(existingStock));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            await _stockRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}