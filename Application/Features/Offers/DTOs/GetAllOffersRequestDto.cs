namespace Application.Features.Offers.DTOs
{
    public sealed class GetAllOffersRequestDto
    {
        // --- Filtreleme ---
        public long? MerchantId { get; set; }
        public long? ProgramId { get; set; }
        public bool? InStock { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Search { get; set; }

        // --- Sayfalama ---
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = (value <= 0) ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value <= 0 || value > 100) ? 10 : value;
        }

        // --- Sıralama ---
        public string? SortBy { get; set; } = "PriceAmount"; // varsayılan sütun
        public string? SortOrder { get; set; } = "asc";      // "asc" veya "desc"
    }
}
