using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
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
            string fileNameChange;
            if (!(og.FileName == updated.FileName))
            {
                fileNameChange = $"{og.FileName} --> {updated.FileName}";
            }
            else
            {
                fileNameChange = $"{og.FileName}";
            }

            //Read file 1
            StreamReader reader1 = new StreamReader(og.OpenReadStream());
            string file1Content = await reader1.ReadToEndAsync();

            //Read file 2
            StreamReader reader2 = new StreamReader(updated.OpenReadStream());
            string file2Content = await reader2.ReadToEndAsync();

            //Some how get the differences by my own

            var differences = GetDifferences(file1Content, file2Content);

            return Ok(differences);
        }

        private string GetDifferences(string content1, string content2)
        {
            // separate line by line logic
            var original = content1.Split("\n");
            var updated = content2.Split("\n");

            // get max length to use on loop
            int maxLengthLines = Math.Max(original.Length, updated.Length);

            StringBuilder sbLineDifferences = new StringBuilder();

            StringBuilder sbWordDifferences = new StringBuilder();

            for (int i = 0; i < maxLengthLines; i++)
            {
                // to avoid out of index error if anyone of those is out of index return No Line
                var originalLine = i < original.Length ? original[i] : "No Line";
                var updatedLine = i < updated.Length ? updated[i] : "No Line";

                // if updated is different than original = difference
                if (updatedLine != originalLine)
                {
                    sbLineDifferences.AppendLine($"<p id=\"new\">{updatedLine}</p>");

                    //separate word by word
                    var originalWord = originalLine.Split(" ");
                    var updatedWord = updatedLine.Split(" ");

                    //
                    int maxLengthWord = Math.Max(originalWord.Length, updatedWord.Length);

                    for (int j = 0; j < maxLengthWord; j++)
                    {
                        var word1 = j < originalWord.Length ? originalWord[j] : " ";
                        var word2 = j < updatedWord.Length ? updatedWord[j] : " ";

                        if (word2 != word1)
                        {
                            sbWordDifferences.AppendLine($"{word2}");
                        }
                    }
                }
            }

            return sbLineDifferences.ToString();
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
