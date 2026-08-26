using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images.Rename;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class Rename : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("images/rename", async ( [FromBody] Request request, [FromServices] ISender sender) =>
        {
            RenameCommand command = new(request.ImageId, request.NewName);
            Result result = await sender.Send(command);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Image);
    }
    public sealed class Request
    {
        public Guid ImageId { get; set; }
        public string NewName { get; set; }
    }
}
