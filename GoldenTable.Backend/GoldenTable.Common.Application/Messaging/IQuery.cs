using GoldenTable.Common.Domain;
using MediatR;

namespace GoldenTable.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
