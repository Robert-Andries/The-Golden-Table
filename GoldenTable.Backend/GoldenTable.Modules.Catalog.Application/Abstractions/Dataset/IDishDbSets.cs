using GoldenTable.Modules.Catalog.Domain.Dishes;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;

public interface IDishDbSets
{
    DbSet<Dish> Dishes { get; }
}
