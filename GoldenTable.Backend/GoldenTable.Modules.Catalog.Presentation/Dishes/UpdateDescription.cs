using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateDescription;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class UpdateDescription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/update-description/", async (Request request, ISender sender) =>
        {
            Description description = new(request.NewDescription);
            Result result = await sender.Send(new UpdateDescriptionCommand(request.DishId, description));

            return result.Match(Results.NoContent, ApiResults.Problem);
        });
    }

    internal sealed class Request
    {
        public Guid DishId { get; set; }
        public string NewDescription { get; set; }
    }
}
