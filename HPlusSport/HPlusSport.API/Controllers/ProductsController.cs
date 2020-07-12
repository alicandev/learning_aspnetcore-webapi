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

            if (queryParameters.MinPrice != null && queryParameters.MaxPrice != null)
                products = products.Where(p => 
                    p.Price >= queryParameters.MinPrice.Value &&
                    p.Price <= queryParameters.MaxPrice.Value 
                );
            else if (queryParameters.MinPrice != null)
                products = products.Where(p => 
                    p.Price > queryParameters.MinPrice.Value
                );
            else if (queryParameters.MaxPrice != null)
                products = products.Where(p => 
                    p.Price < queryParameters.MaxPrice.Value
                );

            if (!String.IsNullOrEmpty(queryParameters.Sku))
                products = products.Where(p => 
                    p.Sku == queryParameters.Sku
                );
            
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

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }
    }
}