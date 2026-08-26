using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images;
using GoldenTable.Modules.Catalog.Application.Images.GetImageById;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("images/get/{id:guid}", async ( [FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            GetImageByIdQuery command = new(id);
            Result<ImageResponse> result = await sender.Send(command);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Image);
    }
}
