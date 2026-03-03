using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Images.Create;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Images;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("images/create", async (Request request, ISender sender) =>
        {
            CreateImageCommand command = new(request.Uri, request.Name, request.Description);
            Result<Guid> result = await sender.Send(command);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Image);
    }
    public sealed class Request
    {
        public Uri Uri { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
