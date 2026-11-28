using System.Security.Claims;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.ModifyItems;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using GoldenTable.Modules.Orders.Presentation.Orders.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

internal sealed class ModifyItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("orders/modify-items", async ([FromBody] Request request, ClaimsPrincipal user, ISender sender) =>
            {
                Result result = await sender.Send(new ModifyItemsCommand(
                    user.GetUserId(),
                    request.OrderId,
                    OrderItemRequest.ToCommandItems(request.Items)));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequirePermission(Permissions.ModifyItems)
            .WithTags(Tags.Order);
    }

    internal sealed class Request
    {
        public Guid OrderId { get; init; }
        public IReadOnlyCollection<OrderItemRequest>? Items { get; init; }
    }
}
