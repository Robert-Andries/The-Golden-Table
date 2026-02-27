using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateNutritionalInformation;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;
using GoldenTable.Modules.Catalog.Presentation.Dishes.Common.NutritionalInformation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Catalog.Presentation.Dishes;

public abstract class UpdateNutritionalInformation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dishes/update-nutritional-information/", async (Request request, ISender sender) =>
        {
            Result<NutritionalValues> nutritionalValuesResult = NutritionalValues.Create(
                request.NutritionalInfo.Kcal,
                request.NutritionalInfo.GramsOfFat,
                request.NutritionalInfo.GramsOfCarbohydrates,
                request.NutritionalInfo.GramsOfSugar,
                request.NutritionalInfo.GramsOfProtein,
                request.NutritionalInfo.GramsOfSalt);

            if (nutritionalValuesResult.IsFailure)
            {
                return ApiResults.Problem(nutritionalValuesResult.Error);
            }

            NutritionalValues nutritionalValues = nutritionalValuesResult.Value;

            Result result =
                await sender.Send(new UpdateNutritionalInformationCommand(request.DishId, nutritionalValues));

            return result.Match(Results.NoContent, ApiResults.Problem);
        });
    }

    internal sealed class Request
    {
        public Guid DishId { get; set; }
        public NutritionalRequest NutritionalInfo { get; set; }
    }
}
