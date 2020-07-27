using BSTServer.Models;
using BSTServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BSTServer.Controllers.User
{
    [Route("api/users/signup")]
    [ApiController]
    public class SignUpController : JsonController
    {
        private IUserService _userService;

        public SignUpController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/users/signup
        [AllowAnonymous]
        [HttpPost]
        public async ValueTask<IActionResult> Post([FromBody] SignUpModel signUpModel)
        {
            try
            {
                await _userService.AddUser(signUpModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(null);
        }
    }
}
