namespace Orbital.Application.Common.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
    public static NotFoundException For(string resource, object id) =>
        new($"{resource} with id '{id}' was not found.");
}
