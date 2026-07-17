using PromptlyNote.Core.Constants;
using PromptlyNote.Core.Exceptions;

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
                throw new BadRequestException("Invalid page number.");
            }

            if (pageSize < 1)
            {
                throw new BadRequestException($"Page size must be greater than {PaginationConfiguration.MinimumPage}.");
            }

            if (page > PaginationConfiguration.MaxPageSize)
            {
                throw new BadRequestException($"Page size must be less than or equal to {PaginationConfiguration.MaxPageSize}.");
            }
        }
    }
}
