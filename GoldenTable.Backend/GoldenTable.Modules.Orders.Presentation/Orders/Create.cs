using System.Security.Claims;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.Create;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using GoldenTable.Modules.Orders.Presentation.Orders.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("orders/create", async ([FromBody] Request request, ClaimsPrincipal user, ISender sender) =>
            {
                Result<Order> result = await sender.Send(new CreateOrderCommand(
                    user.GetUserId(),
                    OrderItemRequest.ToCommandItems(request.Items),
                    AddressRequest.ToAddress(request.Address)!));

                return result.Match(
                    order => Results.Created($"orders/my-orders/{order.Id}", OrderResponse.FromOrder(order)),
                    ApiResults.Problem);
            })
            .RequirePermission(Permissions.CreateOrder)
            .WithTags(Tags.Order);
    }

    internal sealed class Request
    {
        public IReadOnlyCollection<OrderItemRequest>? Items { get; init; }
        public AddressRequest? Address { get; init; }
    }
}
