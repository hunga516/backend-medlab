using System.ComponentModel.DataAnnotations.Schema;

namespace api_sixOs.Models;

public class TestingPackage
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Img { get; set; } 
    
    public string Category { get; set; }
    
    public string? Status { get; set; } = "";
    
    public DateOnly CreatedAt { get; set; }
    [NotMapped] 
    public IFormFile? ImageFile { get; set; }


    public int ReasonId { get; set; } = int.MaxValue;
    
    public Reason Reason { get; set; }
    
    public int PurposeId { get; set; } = int.MaxValue;
    
    public Purpose Purpose { get; set; }


}