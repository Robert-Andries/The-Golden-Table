using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.AddSize;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

internal sealed class AddSize : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dishes/size/add", async (Request request, ISender sender) =>
            {
                Result result = await sender.Send(new AddSizeCommand(
                    request.DishId,
                    request.SizeName,
                    request.SizePriceAdded,
                    request.SizeWeight));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .WithTags(Tags.Dish);
    }

    internal sealed class Request
    {
        public Guid DishId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public float SizePriceAdded { get; set; }
        public float SizeWeight { get; set; }
    }
}
