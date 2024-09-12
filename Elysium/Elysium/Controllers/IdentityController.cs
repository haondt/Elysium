﻿using Elysium.Client.Hubs;
using Elysium.Components.Components;
using Haondt.Web.Core.Controllers;
using Haondt.Web.Core.Extensions;
using Haondt.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Elysium.Controllers
{
    [Route("identity")]
    public class IdentityController(IPageComponentFactory pageFactory, IHubContext<ElysiumHub> hub) : BaseController
    {
        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var result = await pageFactory.GetComponent<LoginModel>();
            return result.CreateView(this);
        } 
    }
}
