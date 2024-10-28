using System;
using FS.DAL.Interfaces;
using FS.DAL.Queries;
using Microsoft.EntityFrameworkCore;

namespace FS.DAL.Implements;

public class GenericeRepository<T, TContext> : IGenericRepository<T, TContext> where T : class where TContext : DbContext
{
    private readonly TContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericeRepository(TContext context)
    {
        this._context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task CreateAllAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public Task UpdateAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);

        return Task.CompletedTask;
    }

    public Task DeleteAllAsync(List<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public IQueryable<T> Get(QueryOptions<T> options)
    {
        IQueryable<T> query = _dbSet;

        if (options.Tracked)
        {
            query = query.AsNoTracking();
        }

        if (options.IncludeProperties?.Any() ?? false)
        {
            foreach (var includeProperty in options.IncludeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        if (options.Predicate != null)
        {
            query = query.Where(options.Predicate);
        }

        if (options.OrderBy != null)
        {
            query = options.OrderBy(query);
        }

        return query;
    }

    public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T> options)
    {
        return await Get(options).ToListAsync();
    }

    public async Task<T> GetSingleAsync(QueryOptions<T> options)
    {
        return await Get(options).FirstOrDefaultAsync();
    }

    public async Task<bool> AnyAsync(QueryOptions<T> options)
    {
        if (options.Predicate != null)
        {
            return await _dbSet.AnyAsync(options.Predicate);
        }
        return false;
    }
}
