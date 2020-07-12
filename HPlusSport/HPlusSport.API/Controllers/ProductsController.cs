using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HPlusSport.API.Models;
using Microsoft.AspNetCore.Mvc;

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
            
            // Check whether the database is created as the Seed is not an
            // automatic process. This seeds the database if it's not yet seeded.
            _context.Database.EnsureCreated(); 
        }
        
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            return Ok(
                _context.Products.ToArray()
            );
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            return Ok(
                _context.Products.Find(id)
            );
        }
    }
}