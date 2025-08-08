using System.ComponentModel.DataAnnotations;

namespace APIDiff.Models
{
    public class RequestFileModel
    {
        [Required]
        public required IFormFile File1 { get; set; }

        [Required]
        public required IFormFile File2 { get; set; }
    }
}
