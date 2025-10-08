namespace PracticeApi.Application.Common.Response
{
    // Pagination 전용 responseDTO
    public class PagedResult<T>
    {
        // 살제 data가 들어있는 field
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        // 이하: pagination 메타데이터
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPage => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}