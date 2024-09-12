using Microsoft.AspNetCore.Mvc;

namespace Elysium.Silo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello!");
        }
    }
}
