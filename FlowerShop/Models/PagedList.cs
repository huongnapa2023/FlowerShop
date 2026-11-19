using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PagedList
{
    public interface IPagedList : IEnumerable
    {
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        int FirstItemOnPage { get; }
        int LastItemOnPage { get; }
    }

    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {
        T this[int index] { get; }
        int Count { get; }
    }

    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get { return PageNumber > 1; } }
        public bool HasNextPage { get { return PageNumber < PageCount; } }
        public bool IsFirstPage { get { return PageNumber == 1; } }
        public bool IsLastPage { get { return PageNumber >= PageCount; } }
        public int FirstItemOnPage { get { return TotalItemCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1; } }
        public int LastItemOnPage { get { return TotalItemCount == 0 ? 0 : Math.Min(PageNumber * PageSize, TotalItemCount); } }

        public PagedList(IQueryable<T> superset, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalItemCount = superset.Count();
            PageCount = TotalItemCount > 0
                ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
                : 1;

            if (PageNumber > PageCount)
            {
                PageNumber = PageCount;
            }

            if (TotalItemCount > 0)
            {
                AddRange(superset.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList());
            }
        }

        public PagedList(IEnumerable<T> superset, int pageNumber, int pageSize)
        {
            var list = superset.ToList();
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalItemCount = list.Count;
            PageCount = TotalItemCount > 0
                ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
                : 1;

            if (PageNumber > PageCount)
            {
                PageNumber = PageCount;
            }

            if (TotalItemCount > 0)
            {
                AddRange(list.Skip((PageNumber - 1) * PageSize).Take(PageSize));
            }
        }
    }

    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> superset, int pageNumber, int pageSize)
        {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }
    }
}
