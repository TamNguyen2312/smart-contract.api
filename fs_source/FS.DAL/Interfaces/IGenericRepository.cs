using System;
using System.Data.Common;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace FS.DAL.Interfaces;

public interface IGenericRepository<T, TContext> where T : class where TContext : DbContext
{
    public Task<T> CreateAsync(T entity);
    public Task CreateAllAsync(List<T> entities);
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity);
    public Task DeleteAllAsync(List<T> entities);
    public IQueryable<T> Get(QueryOptions<T> options);
    public Task<IEnumerable<T>> GetAllAsync(QueryOptions<T> options);
    public Task<T> GetSingleAsync(QueryOptions<T> options);
    public Task<bool> AnyAsync(QueryOptions<T> options);
    public DbSet<T> GetDbSet();
}
