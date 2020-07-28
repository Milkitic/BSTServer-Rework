using BSTServer.Explorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BSTServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExplorerController : JsonController
    {
        private readonly FileExplorer _fileExplorer;

        public ExplorerController(FileExplorer fileExplorer)
        {
            _fileExplorer = fileExplorer;
        }

        // GET: api/<ExplorerController>
        [HttpGet]
        public async Task<IActionResult> Get(char splitChar, string relativePath)
        {
            var userName = User.Claims.FirstOrDefault(k => k.Type == ClaimTypes.Name);
            var userRole = User.Claims.FirstOrDefault(k => k.Type == ClaimTypes.Role);
            try
            {
                var dir = _fileExplorer.GetTargetDirectoryInfo(userRole?.Value, userName?.Value, relativePath?.Split(splitChar));
                return Ok(dir);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<ExplorerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ExplorerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ExplorerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExplorerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
