using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images.EditImage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class EditImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("images/edit/{id}", async (Guid id, [FromBody] Request request, [FromServices] ISender sender) =>
            {
                Result result = await sender.Send(new EditImageCommand(
                    id,
                    request.Name,
                    request.Description,
                    request.Uri));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .WithTags(Tags.Image);
    }

    internal sealed class Request
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Uri Uri { get; set; }
    }
}
