using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.RemoveTags;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class RemoveTags : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/remove-tags/", async (Request request, ISender sender) =>
        {
            Result result = await sender.Send(new RemoveTagsCommand(request.DishId, request.Tags));

            return result.Match(Results.NoContent, ApiResults.Problem);
        });
    }

    internal sealed class Request
    {
        public Guid DishId { get; set; }
        public List<DishTag> Tags { get; set; }
    }
}
