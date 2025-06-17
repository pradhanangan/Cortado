using Customers.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Common.Abstraction;

namespace Customers.Infrastructure.Persistance;

public class CustomersDbContext : DbContext, ICustomersDbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries()
            .Where(e => e.Entity is BaseAuditableEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified)
            ))
        {
            entry.Property("LastModifiedDate").CurrentValue = timestamp;
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedDate").CurrentValue = timestamp;
            }
        }

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw;
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => await Database.BeginTransactionAsync(cancellationToken);

    public void ClearTrackingChanges() => ChangeTracker.Clear();
}
