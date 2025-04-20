using Bookings.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Common.Abstraction;

namespace Bookings.Infrastructure.Persistance;

public class BookingsDbContext : DbContext, IBookingsDbContext
{
    public BookingsDbContext(DbContextOptions<BookingsDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("ticket_number_seq")
            .StartsAt(100000000L)
            .IncrementsBy(1);

        //modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingsDbContext).Assembly);
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

    public void ClearTrackingChanges() => ChangeTracker.Clear();
    
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        => await Database.BeginTransactionAsync(cancellationToken);
}
