using System.ComponentModel.DataAnnotations.Schema;

namespace api_sixOs.Models;

public class Blogs
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Img { get; set; } 
    
    public string Category { get; set; }

    public int ViewCount { get; set; } = 0;
    
    public string? Status { get; set; } = "";
    
    public DateOnly CreatedAt { get; set; }
    [NotMapped] 
    public IFormFile? ImageFile { get; set; } 
}
