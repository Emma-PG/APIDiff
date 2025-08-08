using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace APIDiff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileComparer : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> HtmlFileComparer([FromForm] FilesToCompare requestedFiles)
        {
            //file validation
            if (requestedFiles.File1 == null || requestedFiles.File2 == null)
            {
                return BadRequest("Both files are required.");
            }

            //difference between file names
            IFormFile og = requestedFiles.File1;
            IFormFile updated = requestedFiles.File2;

            //Read file 1
            StreamReader reader1 = new StreamReader(og.OpenReadStream());
            string file1Content = await reader1.ReadToEndAsync();

            //Read file 2
            StreamReader reader2 = new StreamReader(updated.OpenReadStream());
            string file2Content = await reader2.ReadToEndAsync();

            //Some how get the differences by my own

            var differences = GetDifferences(
                file1Content,
                file2Content,
                og.FileName,
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
            </ body >
            </html>"
            );
        }

        private string[] GetDifferences(
            string content1,
            string content2,
            string originalFileName,
            string updatedFileName
        )
        {
            string FileName;

            if (!(originalFileName == updatedFileName))
            {
                FileName = $"File name changed from {originalFileName} --> {updatedFileName}";
            }
            else
            {
                FileName = $"File name does not changed {originalFileName}";
            }

            // separate line by line logic
            var original = content1.Split("\n");
            var updated = content2.Split("\n");

            // get max length to use on loop
            int maxLengthLines = Math.Max(original.Length, updated.Length);

            StringBuilder sbLineDifferences = new StringBuilder();
            StringBuilder sbLineoriginal = new StringBuilder();

            for (int i = 0; i < maxLengthLines; i++)
            {
                // to avoid out of index error if anyone of those is out of index return No Line
                var originalLine = i < original.Length ? original[i] : "No Line";
                var updatedLine = i < updated.Length ? updated[i] : "No Line";

                if (originalLine != "No Line")
                {
                    sbLineoriginal.AppendLine($"<pre id='old'>{originalLine}</pre>");
                }

                // if updated is different than original = difference
                if (updatedLine != originalLine)
                {
                    sbLineDifferences.AppendLine($"<pre id='new'>{updatedLine}</pre>");
                }
                else
                {
                    sbLineDifferences.AppendLine($"<pre id='same'>{updatedLine}</pre>");
                }
            }

            return [sbLineDifferences.ToString(), sbLineoriginal.ToString(), FileName];
        }
    }

    public class FilesToCompare
    {
        [Required]
        public required IFormFile File1 { get; set; }

        [Required]
        public required IFormFile File2 { get; set; }
    }
}
