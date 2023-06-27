namespace Lesson3.Models.DTO
{
    public class StatementListStudentsDTO
    {
        public bool Success { get; set; }
        public string Information { get; set; }
        public IList<Student> Students { get; set; }
    }
}
