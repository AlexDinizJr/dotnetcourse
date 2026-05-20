using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;
using api.Data;
using api.DTOs.Stock;
using api.Models;
using api.Helpers;

namespace api.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<List<Stock>> GetAllAsync(StockQueryObject stockQuery)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(stockQuery.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(stockQuery.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(stockQuery.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(stockQuery.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(stockQuery.SortBy))
            {
                if (stockQuery.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = stockQuery.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (stockQuery.PageNumber - 1) * stockQuery.PageSize;


            return await stocks.Skip(skipNumber).Take(stockQuery.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDTO stockDto)
        {
            var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockDto.Symbol;
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.Industry = stockDto.Industry;
            existingStock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();

            return existingStock;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
            {
                return null;
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return stock;
        }
        
    }
}