namespace SnapHub.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Title { get; set; } = "Portfolio";
        public List<Photo> Photos { get; set; }
    }
}
