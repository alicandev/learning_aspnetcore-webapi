using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPlusSport.API.Classes;
using HPlusSport.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context; // Dependency inject shop context to this controller using its ctor.
        
        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            var products = _context.Products.AsQueryable();

            // min/max price
            products = 
                (queryParameters.MinPrice, queryParameters.MaxPrice) switch {
                    (null, null) => products,
                    (_, null) => products.Where(p => p.Price > queryParameters.MinPrice.Value),
                    (null, _) => products.Where( p => p.Price < queryParameters.MaxPrice.Value),
                    (_, _) => products.Where(p => 
                        p.Price >= queryParameters.MinPrice.Value 
                        && p.Price <= queryParameters.MaxPrice.Value
                    )
                };

            // sku
            products =
                !string.IsNullOrEmpty(queryParameters.Sku)
                    ? products.Where(p => p.Sku == queryParameters.Sku)
                    : products;

            // search
            products =
                !string.IsNullOrEmpty(queryParameters.Name)
                    ? products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()))
                    : products;
            
            // order
            products =
                !string.IsNullOrEmpty(queryParameters.SortBy)
                && typeof(Product).GetProperty(queryParameters.SortBy) != null
                    ? products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder)
                    : products;
            
            // pagination
            products =
                products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")] 
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null) return NotFound();
            
            return Ok(product);
        }
    }
}