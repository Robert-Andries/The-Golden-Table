using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public class GetByTags : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        EndpointRouteBuilderExtensions.MapGet(app, "dishes/get-dish-by-tags/", 
            async ( [AsParameters] Request request, [FromServices] ISender sender) =>
        {
            var tags = Enumerable.Select(request.Tags, tag => DishTag.Create(tag).Value).ToList();
            
            Result<List<DishResponse>> result = await sender.Send(new GetDishesByTagsQuery(tags));

            return ResultExtensions.Match(result, Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Dish);
    }

    internal sealed class Request
    {
        [FromQuery] 
        public string[] Tags { get; set; }
    }
}
