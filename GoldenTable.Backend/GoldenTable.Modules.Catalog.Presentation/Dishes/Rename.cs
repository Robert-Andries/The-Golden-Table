using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.Rename;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class Rename : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("dishes/rename/", async ( [FromBody] Request request, [FromServices] ISender sender) =>
        {
            Name name = new(request.NewName);
            Result result = await sender.Send(new RenameCommand(request.DishId, name));

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Dish);
    }

    internal sealed class Request
    {
        public Guid DishId { get; set; }
        public string NewName { get; set; }
    }
}
