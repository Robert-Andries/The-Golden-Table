using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images.Delete;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("images/delete/{id:guid}", async ( [FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            DeleteImageCommand command = new(id);
            Result result = await sender.Send(command);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Image);
    }
}
