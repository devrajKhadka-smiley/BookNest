namespace BookNest.Models.Dto
{
    public class StaffOrderDto
    {
        public string? MembershipId { get; set; }
        public Guid OrderId { get; set; }
        public int ClaimCode { get; set; }
    }
}
