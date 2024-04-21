namespace ChallengeApp.Application.Common.Exceptions;

public class UnauthorizeException : Exception
{
    public UnauthorizeException()
        : base()
    {
    }

    public UnauthorizeException(string message)
        : base(message)
    {
    }

    public UnauthorizeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UnauthorizeException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}