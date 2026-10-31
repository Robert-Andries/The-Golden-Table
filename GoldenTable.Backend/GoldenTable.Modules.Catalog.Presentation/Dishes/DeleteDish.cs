using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;
using GoldenTable.Modules.Catalog.Presentation.Authentication;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

internal sealed class DeleteDish : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("dishes/delete/{id:guid}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
            {
                DeleteDishCommand command = new(id);
                Result result = await sender.Send(command);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequirePermission(Permissions.DeleteDish)
            .WithTags(Tags.Dish);
    }
}
