using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.GetAllDishes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/get-all", async (ISender sender) =>
        {
            Result<List<Dish>> result = await sender.Send(new GetAllDishesQuery());

            return result.Match(Results.Ok, ApiResults.Problem);
        });
    }
}
