namespace BookNest.Models.Dto
{
    public class StaffOrderDto
    {
        public string? MembershipId { get; set; }
        public Guid OrderId { get; set; }
        public string? ClaimCode { get; set; }
    }
}
