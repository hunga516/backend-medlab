using System.Text.Json.Serialization;

public class Booking
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("bookingDate")]
    public DateOnly? BookingDate { get; set; }
    
    [JsonPropertyName("status")]
    public string? Status { get; set; } = "notSent";
}