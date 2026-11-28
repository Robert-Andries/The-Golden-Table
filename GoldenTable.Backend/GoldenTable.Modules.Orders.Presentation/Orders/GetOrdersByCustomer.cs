using System.Security.Claims;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.GetOrdersByCustomer;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using GoldenTable.Modules.Orders.Presentation.Orders.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

internal sealed class GetOrdersByCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("orders/my-orders", async (ClaimsPrincipal user, ISender sender) =>
            {
                Result<IReadOnlyCollection<Order>> result =
                    await sender.Send(new GetOrdersByCustomerQuery(user.GetUserId()));

                return result.Match(orders => Results.Ok(OrderResponse.FromOrders(orders)), ApiResults.Problem);
            })
            .RequirePermission(Permissions.ViewOwnOrders)
            .WithTags(Tags.Order);
    }
}
