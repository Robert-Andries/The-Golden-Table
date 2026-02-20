using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class GetByTags : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/get-dish-by-tags/", async (Request request, ISender sender) =>
        {
            Result<List<Dish>> result = await sender.Send(new GetDishesByTagsQuery(request.Tags));

            return result.Match(Results.Ok, ApiResults.Problem);
        });
    }

    internal sealed class Request
    {
        public List<DishTag> Tags { get; set; }
    }
}
