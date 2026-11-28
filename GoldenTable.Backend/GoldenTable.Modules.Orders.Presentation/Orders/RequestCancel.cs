using System.Security.Claims;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.RequestCancel;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

internal sealed class RequestCancel : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("orders/request-cancel/{orderId:guid}",
                async ([FromRoute] Guid orderId, ClaimsPrincipal user, ISender sender) =>
                {
                    Result result = await sender.Send(new RequestCancelCommand(user.GetUserId(), orderId));

                    return result.Match(Results.NoContent, ApiResults.Problem);
                })
            .RequirePermission(Permissions.RequestCancel)
            .WithTags(Tags.Order);
    }
}
