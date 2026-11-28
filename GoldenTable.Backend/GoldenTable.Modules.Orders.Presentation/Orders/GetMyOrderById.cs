using System.Security.Claims;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.GetOrderById;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using GoldenTable.Modules.Orders.Presentation.Orders.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

/// <summary>
///     Customer endpoint: reads one of the caller's own orders. Another customer's order returns 404.
/// </summary>
internal sealed class GetMyOrderById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("orders/my-orders/{id:guid}", async ([FromRoute] Guid id, ClaimsPrincipal user, ISender sender) =>
            {
                Result<Order> result = await sender.Send(new GetOrderByIdQuery(id, user.GetUserId()));

                return result.Match(order => Results.Ok(OrderResponse.FromOrder(order)), ApiResults.Problem);
            })
            .RequirePermission(Permissions.ViewOwnOrders)
            .WithTags(Tags.Order);
    }
}
