namespace Orbital.Application.Common.Exceptions;

public sealed class UnauthorizedException(string message = "Unauthorized.") : Exception(message);
