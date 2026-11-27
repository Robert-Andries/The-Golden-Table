using GoldenTable.Common.Domain;
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
///     Staff endpoint: reads any order. Customers use <see cref="GetMyOrderById" />.
/// </summary>
internal sealed class GetOrderById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("orders/get/{id:guid}", async ([FromRoute] Guid id, ISender sender) =>
            {
                Result<Order> result = await sender.Send(new GetOrderByIdQuery(id, CustomerId: null));

                return result.Match(order => Results.Ok(OrderResponse.FromOrder(order)), ApiResults.Problem);
            })
            .RequirePermission(Permissions.GetOrderById)
            .WithTags(Tags.Order);
    }
}
