namespace Application.Features.Offers.DTOs
{
    public sealed class OfferListResponseDto
    {
        public long Id { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public string? ProgramName { get; set; }
        public string? AffiliateUrl { get; set; }
        public string? LandingUrl { get; set; }
        public decimal PriceAmount { get; set; }
        public string Currency { get; set; } = "₺";
        public bool InStock { get; set; }
        public decimal? ShippingFee { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}