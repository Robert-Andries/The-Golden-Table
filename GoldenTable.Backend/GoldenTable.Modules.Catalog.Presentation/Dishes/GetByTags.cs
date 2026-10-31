using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

internal sealed class GetByTags : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/get-dish-by-tags/",
                async ([AsParameters] Request request, [FromServices] ISender sender) =>
                {
                    Result<List<DishResponse>> result =
                        await sender.Send(new GetDishesByTagsQuery(request.TagIds.ToList()));

                    return result.Match(Results.Ok, ApiResults.Problem);
                })
            .AllowAnonymous()
            .WithTags(Tags.Dish);
    }

    internal sealed class Request
    {
        [FromQuery] public Guid[] TagIds { get; set; } = [];
    }
}
