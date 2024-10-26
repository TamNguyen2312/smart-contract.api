using System;
using FS.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FS.DAL.Implements;

public class FSUnitOfWork<TContext> : IFSUnitOfWork<TContext>, IDisposable where TContext : DbContext
{
    private readonly TContext _context;
    private readonly IServiceProvider _serviceProvider;
    private IDbContextTransaction _transaction;

    public FSUnitOfWork(TContext context, IServiceProvider serviceProvider)
    {
        this._context = context;
        this._serviceProvider = serviceProvider;
    }


    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }

    private bool disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }
    }

    public IGenericRepository<TEntity, TContext> GetRepository<TEntity>() where TEntity : class
    {
        return _serviceProvider.GetRequiredService<IGenericRepository<TEntity, TContext>>();
    }

    public async Task RollBackAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null!;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
