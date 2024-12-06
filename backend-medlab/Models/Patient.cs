namespace api_sixOs.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly Dob { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
    }
}
