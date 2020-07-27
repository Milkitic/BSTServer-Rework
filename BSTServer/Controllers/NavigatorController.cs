using BSTServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BSTServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NavigatorController : JsonController
    {
        // GET: api/Navigator
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var claim = User.Claims.First(k => k.Type == ClaimTypes.Role);
            switch (claim.Value)
            {
                case UserRoles.User:
                    return Ok("user!");
                case UserRoles.Admin:
                    return Ok("admin!");
                case UserRoles.Root:
                    return Ok(new NavObj()
                    {
                        Sections = new List<SectionObj>()
                        {
                            SectionObj.CreateGeneral(ItemObj.Dashboard, ItemObj.Statistics),
                            SectionObj.CreateManagement(ItemObj.Server, ItemObj.Files, ItemObj.Users)
                        }
                    });
                default:
                    return BadRequest("unknown user role!");
            }
        }

        // GET: api/Navigator/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Navigator
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Navigator/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
