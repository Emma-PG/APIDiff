using APIDiff.Models;
using APIDiff.Utils;
using Microsoft.AspNetCore.Mvc;

namespace APIDiff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileComparerController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> HtmlFileComparer(
            [FromForm] RequestFileModel requestedFiles
        )
        {
            //file validation
            if (requestedFiles.File1 == null || requestedFiles.File2 == null)
            {
                return BadRequest("Both files are required.");
            }
            if (
                requestedFiles.File1.ContentType != "text/plain"
                || requestedFiles.File2.ContentType != "text/plain"
            )
            {
                return BadRequest("Both files should be text/plain based.");
            }
            if (requestedFiles.File1.Length == 0 || requestedFiles.File2.Length == 0)
            {
                return BadRequest("Both files should have content");
            }

            //File names
            IFormFile original = requestedFiles.File1;
            IFormFile updated = requestedFiles.File2;

            //File content
            StreamReader reader1 = new StreamReader(original.OpenReadStream());
            string originalContent = await reader1.ReadToEndAsync();

            StreamReader reader2 = new StreamReader(updated.OpenReadStream());
            string updatedContent = await reader2.ReadToEndAsync();

            //Get differences
            var differences = FileComparer.GetDifferences(
                originalContent,
                updatedContent,
                original.FileName,
                updated.FileName
            );

            return Ok(
                @$"
            <!DOCTYPE html>
            <html>
                <head>
                    <title>File Differences</title>
                </head>
                <body>
                    <h2>{differences[2]}</h2>
                    <div id='container'>
                        <div id='newText'>
                            {differences[0]}
                        </div>
                        <div id='oldText'>
                            {differences[1]}
                        </div>
                    </div>
                </body>
            </html>"
            );
        }
    }
}
