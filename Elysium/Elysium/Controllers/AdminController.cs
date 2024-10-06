using Elysium.Components.Components.Admin;
using Haondt.Web.Core.Controllers;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Controllers
{
    [Route("admin")]
    public class AdminController(IPageComponentFactory pageFactory) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAdminPanel()
        {
            var result = await pageFactory.GetComponent<AdminPanelLayoutModel>();
            return result.CreateView(this);
        }
    }
}
