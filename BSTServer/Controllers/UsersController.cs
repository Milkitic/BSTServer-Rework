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
    public class UsersController : JsonController
    {
        private IUserService _userService;
        //private CaptchaService _captchaService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            var user = await _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                //_captchaService.Count(Request.Headers["Identity"]);
                return BadRequest("Username or password is incorrect");
            }

            return Ok(new
            {
                user,
                principal = User.Identities.Select(k => new
                {
                    k.Name,
                    k.AuthenticationType,
                    claims = k.Claims.ToDictionary(o => o.Type, o => o.Value)
                })
            });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetCaptcha()
        //{
        //    var head
        //    var s = User;
        //    //var users = await _userService.GetAll();
        //    //return Ok(new
        //    //{
        //    //    users = users,
        //    //    principal = User.Identities.Select(k => new
        //    //    {
        //    //        k.Name,
        //    //        k.AuthenticationType,
        //    //        claims = k.Claims.ToDictionary(o => o.Type, o => o.Value)
        //    //    })
        //    //});
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
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

    public class JsonController : ControllerBase
    {
        public BadRequestObjectResult BadRequest(string message)
        {
            return BadRequest(message, null);
        }
        public BadRequestObjectResult BadRequest(string message, object data)
        {
            return base.BadRequest(new
            {
                code = 400,
                message = message,
                data = data
            });
        }

        public OkObjectResult PartialOk(string message, object data)
        {
            return base.Ok(new
            {
                code = 202,
                message = message,
                data = data
            });
        }

        /// <inheritdoc />
        public override OkObjectResult Ok(object data)
        {
            return base.Ok(new
            {
                code = 200,
                message = (string)null,
                data = data
            });
        }
    }
}