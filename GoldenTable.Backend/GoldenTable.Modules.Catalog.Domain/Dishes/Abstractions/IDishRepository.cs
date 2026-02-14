namespace GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

public interface IDishRepository
{
    Task AddAsync(Dish dish, CancellationToken cancellationToken = default);
    Task<Dish?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Dish>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default);
}
