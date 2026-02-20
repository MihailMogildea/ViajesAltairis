using Microsoft.EntityFrameworkCore;
using ViajesAltairis.Data.Context;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Data.Repositories;

public class SimpleRepository<T> : ISimpleRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public SimpleRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync([id], cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }
}
