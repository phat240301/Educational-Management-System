using EduManagement.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduManagement.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEntity>(EduDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    protected readonly EduDbContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual Task<TEntity?> GetByIdAsync(long id, CancellationToken ct = default) =>
        DbSet.FindAsync([id], ct).AsTask();

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().ToListAsync(ct);

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
        await DbSet.AddAsync(entity, ct);

    public virtual void Update(TEntity entity) => DbSet.Update(entity);

    public virtual void Remove(TEntity entity) => DbSet.Remove(entity);

    public virtual Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        Context.SaveChangesAsync(ct);
}
