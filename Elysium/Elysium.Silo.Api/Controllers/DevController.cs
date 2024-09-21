using Elysium.Silo.Api.Attributes;
using Elysium.Silo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Silo.Api.Controllers
{
    [ApiController]
    [DevOnly]
    [Route("_dev")]
    public class DevControllere(IDevHandler handler) : ControllerBase
    {

        /// <summary>
        /// Take an action on behalf of a local actor
        /// </summary>
        /// <returns></returns>
        [HttpPost("as-local")]
        [Consumes("application/ld+json")]
        public async Task<IActionResult> Post([FromBody] DevLocalActivityPayload payload)
        {
            var (_, activity) = await handler.CreateForLocal(payload);
            return Ok(activity);
        }
    }
}
