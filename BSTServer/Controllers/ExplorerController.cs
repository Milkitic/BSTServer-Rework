using BSTServer.Explorer;
using BSTServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UploadStream;

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
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<ExplorerController>
        [HttpPost("upload")]
        [AllowAnonymous]
        //[DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> Post()
        {
            try
            {
                // returns a generic typed model, alternatively non-generic overload if no model binding is required
                UploadModel model = await this.StreamFiles<UploadModel>(async formFile =>
                {
                    var buffer = new byte[4096];
                    int i = 0;
                    // implement processing of stream as required via an IFormFile interface
                    var createdFile = Path.Combine("e:\\test\\" + formFile.FileName);

                    //using (var readStream = formFile.OpenReadStream())
                    using (var stream = new FileStream(createdFile, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                });
                // ModelState is still validated from model
                if (!ModelState.IsValid)
                {


                }
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Null encoding")
                {
                    return BadRequest("未选择上传文件");
                }

                return BadRequest(ex.Message);
                throw;
            }

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
