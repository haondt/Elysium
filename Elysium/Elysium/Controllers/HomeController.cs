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
            return result.Value.CreateView(this);
        } 
    }
}
