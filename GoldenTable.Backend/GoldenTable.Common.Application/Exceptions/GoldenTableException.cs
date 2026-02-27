using GoldenTable.Common.Domain;

namespace GoldenTable.Common.Application.Exceptions;

public sealed class GoldenTableException : Exception
{
    public GoldenTableException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public Error? Error { get; }
}
