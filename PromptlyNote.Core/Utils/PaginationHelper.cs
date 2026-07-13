using PromptlyNote.Core.Constants;

namespace PromptlyNote.Core.Utils
{
    public static class PaginationHelper
    {
        public static (int page, int pageSize) Normalize(int page, int pageSize)
        {
            page = page < PaginationConfiguration.MinimumPage ? PaginationConfiguration.MinimumPage : page;
            pageSize = pageSize < 1 ? PaginationConfiguration.DefaultPageSize : pageSize;
            pageSize = pageSize > PaginationConfiguration.MaxPageSize ? PaginationConfiguration.MaxPageSize : pageSize;
            return (page, pageSize);
        }

        public static void ValidatePageSettings(int page, int pageSize)
        {
            if (page < PaginationConfiguration.MinimumPage)
            {
                throw new ArgumentException("Invalid page number.", nameof(page));
            }

            if (pageSize < 1)
            {
                throw new ArgumentException($"Page size must be greater than {PaginationConfiguration.MinimumPage}.", nameof(pageSize));
            }

            if (page > PaginationConfiguration.MaxPageSize)
            {
                throw new ArgumentException($"Page size must be less than or equal to {PaginationConfiguration.MaxPageSize}.", nameof(pageSize));
            }
        }
    }
}
