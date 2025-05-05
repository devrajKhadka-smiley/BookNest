namespace BookNest.Models.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public string? MemberShipId { get; set; }
    }
}
