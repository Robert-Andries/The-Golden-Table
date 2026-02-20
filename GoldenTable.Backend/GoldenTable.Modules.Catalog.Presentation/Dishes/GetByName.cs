using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByName;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class GetByName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/name/{name}", async (string name, ISender sender) =>
        {
            Name finalName = new(name);

            Result<List<Dish>> result = await sender.Send(new GetDishesByNameQuery(finalName));

            return result.Match(Results.Ok, ApiResults.Problem);
        });
    }
}
