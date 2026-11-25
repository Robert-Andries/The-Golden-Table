using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Common.Infrastructure.Outbox;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Infrastructure.Dishes;
using GoldenTable.Modules.Orders.Infrastructure.Orders;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Orders.Infrastructure.Database;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Order> Orders { get; set; }

    /// <summary>The Orders module's copy of the Catalog dishes, kept in sync through integration events</summary>
    public DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Orders);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new DishConfiguration());
        modelBuilder.Entity<Order>().Ignore(o => o.DomainEvents);
        modelBuilder.Entity<Dish>().Ignore(d => d.DomainEvents);
    }
}
