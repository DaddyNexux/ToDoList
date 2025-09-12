


using ToDoList.Helpers;
using ToDoList.Models.DTOs;

namespace ToDoList.Extentions;

public static class DbSetExtensions
{
    public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> query, BaseFilter filter)
    {
        if (query == null)
            return null;

        return await PagedList<T>.Create(query, filter.PageNumber, filter.PageSize);
    }
}