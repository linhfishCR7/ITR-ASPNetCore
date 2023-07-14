using System.ComponentModel.DataAnnotations;

namespace UploadS3.Models
{
    public class FileEntry
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PresignedUrl { get; set; }
    }
}
