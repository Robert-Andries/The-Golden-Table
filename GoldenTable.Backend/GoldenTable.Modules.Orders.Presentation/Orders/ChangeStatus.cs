using GoldenTable.Common.Domain;
using GoldenTable.Common.Presentation.Endpoints;
using GoldenTable.Common.Presentation.Results;
using GoldenTable.Modules.Orders.Application.Orders.ChangeStatus;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Presentation.Authentication;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Modules.Orders.Presentation.Orders;

/// <summary>
///     Staff endpoint: moves any order to a new status, so there is no ownership check
/// </summary>
internal sealed class ChangeStatus : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("orders/change-status", async ([FromBody] Request request, ISender sender) =>
            {
                Result result = await sender.Send(new ChangeStatusCommand(request.OrderId, request.NewOrderStatus));

                return result.Match(Results.NoContent, ApiResults.Problem);
            })
            .RequirePermission(Permissions.ChangeStatus)
            .WithTags(Tags.Order);
    }

    internal sealed class Request
    {
        public Guid OrderId { get; init; }
        public OrderStatus NewOrderStatus { get; init; }
    }
}
