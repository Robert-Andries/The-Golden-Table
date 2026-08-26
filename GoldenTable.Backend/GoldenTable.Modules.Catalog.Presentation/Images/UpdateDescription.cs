using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images.UpdateDescription;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class UpdateDescription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("images/update-description", async ( [FromBody] Request request, [FromServices] ISender sender) =>
            {
                UpdateDescriptionCommand command = new(request.ImageId, request.NewDescription);
                Result result = await sender.Send(command);

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .WithTags(Tags.Image);
    }

    public sealed class Request
    {
        public Guid ImageId { get; set; }
        public string NewDescription { get; set; }
    }
}
