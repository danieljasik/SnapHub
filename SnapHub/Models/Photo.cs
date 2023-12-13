namespace SnapHub.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }

        public int SessionId { get; set; }
    }
}
