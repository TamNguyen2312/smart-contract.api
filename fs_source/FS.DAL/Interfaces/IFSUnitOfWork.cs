using System;
using Microsoft.EntityFrameworkCore;

namespace FS.DAL.Interfaces;

public interface IFSUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    IGenericRepository<T, TContext> GetRepository<T>() where T : class;
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollBackAsync();
    Task<bool> SaveAsync();
}
