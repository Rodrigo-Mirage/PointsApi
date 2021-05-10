using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PointsApi.Models;

namespace PointsApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly PointsContext _context;
        
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger,PointsContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetList")]
        public ActionResult<IEnumerable<Product>> GetList()
        {
            return _context.Products;
        }
    }
}
