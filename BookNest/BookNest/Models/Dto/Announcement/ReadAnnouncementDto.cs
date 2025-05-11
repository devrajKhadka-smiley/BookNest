namespace BookNest.Models.Dto.Announcement
{
    public class ReadAnnouncementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
