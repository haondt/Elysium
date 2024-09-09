using Elysium.WebFinger.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.WebFinger.Controllers
{
    [ApiController]
    [Route("/.well-known/webfinger")]
    public class WebFingerController(IWebFingerService webFingerService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] WebFingerQuery query)
        {
            //var jrd = await webFingerService.GetAsync(query.Resource);
            //if (!jrd.IsSuccessful)
            //    return NotFound();

            //if (query.Rel != null)
            //    jrd.Value.Links = jrd.Value.Links.Where(l => query.Rel.Contains(l.Rel)).ToList();

            //return Ok(jrd.Value);
            throw new NotImplementedException();
        }
    }
}
