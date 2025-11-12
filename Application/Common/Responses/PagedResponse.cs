namespace Application.Common.Responses
{
    public sealed class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public MetaData Meta { get; set; } = new();

        public sealed class MetaData
        {
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public int TotalPages { get; set; }
        }
    }
}
