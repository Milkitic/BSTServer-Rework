using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BSTServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BSTServer.Controllers.User
{
    [Authorize]
    [Route("api/user/logout")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private IUserService _userService;

        public LogoutController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user/logout
        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            //await HttpContext.SignOutAsync();
            return Ok();
        }


        // POST: api/user/logout
        [HttpPost]
        public async ValueTask<IActionResult> Post()
        {
            return Ok();
        }
    }
}
