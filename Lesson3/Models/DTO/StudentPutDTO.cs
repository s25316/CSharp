namespace Lesson3.Models.DTO
{
    public class StudentPutDTO
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }
        public string MothersName { get; set; }
        public string FathersName { get; set; }
        public Study Studies { get; set; }
    }
}
