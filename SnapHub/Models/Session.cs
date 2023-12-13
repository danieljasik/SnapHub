namespace SnapHub.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SessionCode => $"{Id}-{CreatedDate:ddMMyyyy}";

        public List<Photo> Photos { get; set; }
    }
}
