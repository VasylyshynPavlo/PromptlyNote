using System;
using System.Collections.Generic;
using System.Text;

namespace PromptlyNote.Core.Models
{
    public class PagedResult<T>
    {
        public object? Data { get; set; }
        public int? Count { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public bool IsSingle { get; set; }

        public PagedResult(IReadOnlyCollection<T> items, int count, int? page = null, int? totalPages = null)
        {
            Data = items;
            Count = count;
            CurrentPage = page;
            TotalPages = totalPages;
            IsSingle = false;
        }

        public PagedResult(T? item)
        {
            Data = item;
            Count = item != null ? 1 : 0;
            IsSingle = true;
        }

        public override string ToString()
        {
            return $"PagedResult: Data={Data}, Count={Count}, Page={CurrentPage}, TotalPages={TotalPages}, IsSingle={IsSingle}";
        }
    }
}
