using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.GetOrders;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using GoldenTable.Modules.Orders.Presentation.Orders.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

internal sealed class GetOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("orders/get-all", async (ISender sender) =>
            {
                Result<IReadOnlyCollection<Order>> result = await sender.Send(new GetOrdersQuery());

                return result.Match(orders => Results.Ok(OrderResponse.FromOrders(orders)), ApiResults.Problem);
            })
            .RequirePermission(Permissions.GetAllOrders)
            .WithTags(Tags.Order);
    }
}
