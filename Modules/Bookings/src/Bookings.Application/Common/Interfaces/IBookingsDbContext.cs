using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bookings.Application.Common.Interfaces;

public interface IBookingsDbContext
{
    EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        where TEntity : class;

    EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        where TEntity : class;

    EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        where TEntity : class;

    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        where TEntity : class;

    ChangeTracker ChangeTracker { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    void ClearTrackingChanges();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}