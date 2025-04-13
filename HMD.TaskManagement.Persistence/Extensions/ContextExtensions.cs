using HMD.TaskManagement.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Persistence.Extensions
{
    public static class ContextExtensions
    {
        public static async Task<PagedData<T>> ToPagedAsync<T>(this IQueryable<T> query, int activePage, int pageSize) where T : class, new()
        {
            var list = await query.AsNoTracking().Skip((activePage - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalPage = query.AsNoTracking().Count();

            return new PagedData<T>(list, activePage, totalPage, pageSize);
        }
    }
}
