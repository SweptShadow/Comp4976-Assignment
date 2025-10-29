namespace frontend.Models
{
    public class Obituary
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfDeath { get; set; }
        public string? Biography { get; set; }
        public string? PhotoPath { get; set; }
        public string? SubmittedByName { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
