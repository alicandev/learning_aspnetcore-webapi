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
        public async Task<IActionResult> GetAllProducts([FromQuery] QueryParameters queryParameters)
        {
            var products = 
                _context.Products
                .Skip(queryParameters.Size * queryParameters.Page - 1)
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