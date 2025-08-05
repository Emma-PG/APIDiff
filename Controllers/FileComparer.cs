using Microsoft.AspNetCore.Mvc;

namespace APIDiff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileComparer : ControllerBase
    {
        [HttpPost]
        public string HtmlFileComparer([FromBody] IFormFile og)
        {
            return $"{og.FileName}";
        }
    }
}
