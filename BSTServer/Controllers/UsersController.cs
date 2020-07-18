using BSTServer.Models;
using BSTServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BSTServer.Controllers
{
    [Authorize(Roles = UserRoles.Admin + "," +
                       UserRoles.Root)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
        {
            var user = await _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var s = User;
            var users = await _userService.GetAll();
            return Ok(new
            {
                users = users,
                principal = User.Identities.Select(k => new
                {
                    k.Name,
                    k.AuthenticationType,
                    claims = k.Claims.ToDictionary(o => o.Type, o => o.Value)
                })
            });
        }
    }
}