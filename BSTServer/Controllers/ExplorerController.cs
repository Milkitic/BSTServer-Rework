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
                var dir = _fileExplorer.GetTargetDirectoryInfo(userRole?.Value, userName?.Value,
                    relativePath?.Split(splitChar));
                return Ok(dir);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<ExplorerController>
        [HttpPost("upload")]
        [RequestSizeLimit(long.MaxValue)]
        [AllowAnonymous]
        [RequestFormLimits(BufferBody = true)]
        public async Task<IActionResult> Post()
        {
            try
            {
                // returns a generic typed model, alternatively non-generic overload if no model binding is required
                UploadModel model = await this.StreamFiles<UploadModel>(async formFile =>
                {
                    var fixedFilename = Path.GetFileName(formFile.FileName);

                    string[] supportedExt = { ".vpk", ".zip" };
                    string ext = Path.GetExtension(fixedFilename);
                    if (!supportedExt.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    {
                        throw new NotSupportedException($"File type \"{ext}\" is not supported.");
                    }

                    char[] reverseChar = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToArray();
                    if (fixedFilename?.Any(c => c > 127 && c < 32) != false ||
                        fixedFilename.Any(c => reverseChar.Contains(c)))
                    {
                        throw new NotSupportedException(
                            "File name is invalid. Should be characters or system-support path symbols.");
                    }

                    var fileName = Guid.NewGuid() + "_" + fixedFilename;
                    Console.WriteLine("get: " + fileName);
                    // implement processing of stream as required via an IFormFile interface
                    var testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testFiles");
                    if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);
                    var createdFile = Path.Combine(testDir, fileName);

                    //var buffer = new byte[4096];
                    //int i = 0;
                    //await using var fileStream = new FileStream(createdFile, FileMode.Create, FileAccess.Write);
                    //int bytesRead;
                    //var readStream = formFile.OpenReadStream();
                    //while ((bytesRead = await readStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    //{
                    //    await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                    //    buffer = new byte[4096];
                    //}

                    await using (var stream = new FileStream(createdFile, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    Console.WriteLine("write done: " + fileName);
                });
                // ModelState is still validated from model
                if (!ModelState.IsValid)
                {
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (ex.Message == "Null encoding")
                {
                    return BadRequest("未选择上传文件");
                }

                return BadRequest(ex.Message);
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