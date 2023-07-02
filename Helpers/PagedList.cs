using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SocialApp.Helpers
{
    public class PagedList<T>: List<T>
    {
        public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int count)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var items = await source.Skip((pageNumber -1) * pageSize).Take(pageSize).ToListAsync();
            var count = await source.CountAsync();
            return new PagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}
