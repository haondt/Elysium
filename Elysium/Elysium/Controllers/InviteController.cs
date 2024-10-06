using Elysium.Components.Components;
using Haondt.Web.Core.Controllers;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elysium.Controllers
{
    [Route("invite")]
    public class InviteController(IPageComponentFactory pageFactory) : BaseController
    {
        [HttpGet("{inviteId}")]
        public async Task<IActionResult> Get(string inviteId)
        {
            var result = await pageFactory.GetComponent<InvitedRegisterLayoutModel>(new Dictionary<string, string>
            {
                {  "inviteId",  inviteId },
            });
            return result.CreateView(this);
        }
    }
}
