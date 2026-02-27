using Microsoft.AspNetCore.Routing;

namespace GoldenTable.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
