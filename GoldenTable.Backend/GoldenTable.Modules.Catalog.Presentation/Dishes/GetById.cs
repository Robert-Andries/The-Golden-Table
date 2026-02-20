using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/id/{id}", async (Guid id, ISender sender) =>
        {
            Result<Dish> result = await sender.Send(new GetDishByIdQuery(id));

            return result.Match(Results.Ok, ApiResults.Problem);
        });
    }
}
