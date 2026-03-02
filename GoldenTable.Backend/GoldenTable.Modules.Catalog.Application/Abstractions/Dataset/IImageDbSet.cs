using GoldenTable.Modules.Catalog.Domain.Common.Image;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;

public interface IImageDbSet
{
    DbSet<Image> Images { get; }
}
