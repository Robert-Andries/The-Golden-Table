using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.CreateTag;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

internal sealed class CreateTag : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dishes/tags/create", async ([FromBody] Request request, [FromServices] ISender sender) =>
        {
            CreateTagCommand command = new(request.Value);
            Result<Guid> result = await sender.Send(command);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Dish);
    }

    public sealed class Request
    {
        public string Value { get; set; } = string.Empty;
    }
}
