using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Errors : ControllerBase
    {
        public ActionResult Error (int code)
        {
            if (code == 404)
                return NotFound(new APIsResponse(404));
            else if (code == 401)
                return Unauthorized(new APIsResponse(401));
            else
                return StatusCode(code);
        }
    }
}
