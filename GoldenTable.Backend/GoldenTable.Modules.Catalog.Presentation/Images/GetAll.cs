using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images;
using GoldenTable.Modules.Catalog.Application.Images.GetAll;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("images/get-all/", async ([FromServices] ISender sender) =>
            {
                GetAllImagesQuery command = new();
                Result<List<ImageResponse>> result = await sender.Send(command);

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Image);
    }
}
