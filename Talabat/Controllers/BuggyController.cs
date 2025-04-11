using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : BaseAPIController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("notfound")]
        public IActionResult notFound()
        {
            var product = _dbContext.Products.Find(100);
            if(product == null)
                return NotFound(new APIsResponse(404));
            return Ok(product);
        }
        [HttpGet("badrequest")]
        public IActionResult BadReques()
        {
            return BadRequest(new APIsResponse(400));
        }
        [HttpGet("notauthorization")]
        public IActionResult NotAuthorization()
        {
            return Unauthorized(new APIsResponse(401));
        }
        [HttpGet("{id}")]
        public IActionResult ValidationError(int id)
        {
            var product = _dbContext.Products.Find(id);
            return Ok(product);
        }
        [HttpGet("servererror")]
        public IActionResult ServerError()
        {
            var product = _dbContext.Products.Find(100);
            var productstring = product.ToString();
            return Ok(productstring);
        }
    }
}
