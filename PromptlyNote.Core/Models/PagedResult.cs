namespace PromptlyNote.Core.Models
{
    public class PagedResult<T>
    {
        public IReadOnlyCollection<T> Data { get; set; }
        public int Count { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public PagedResult(IReadOnlyCollection<T> items, int count, int? page = null, int? totalPages = null)
        {
            Data = items;
            Count = count;
            CurrentPage = page;
            TotalPages = totalPages;
        }

        public override string ToString()
        {
            return $"PagedResult: Count={Count}, Page={CurrentPage}, TotalPages={TotalPages}";
        }
    }
}