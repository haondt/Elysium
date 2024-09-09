using Elysium.Components.Components;
using Haondt.Web.Core.Controllers;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Controllers
{
    public class HomeController(IPageComponentFactory pageFactory) : BaseController
    {

        [HttpGet("/")]
        public IActionResult Get()
        {
            return Redirect("/home");
        } 

        [HttpGet("home")]
        public async Task<IActionResult> Home()
        {
            var result = await pageFactory.GetComponent<HomeLayoutModel>();
            return result.CreateView(this);
        } 

        [HttpGet("messages")]
        public async Task<IActionResult> Messages()
        {
            var result = await pageFactory.GetComponent<TemporaryMessageComponentLayoutModel>();
            return result.CreateView(this);
        } 
    }
}
