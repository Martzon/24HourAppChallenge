using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApp.Application.Common.Exceptions;

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException()
        : base()
    {
    }

    public AlreadyExistsException(string message)
        : base(message)
    {
    }

    public AlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public AlreadyExistsException(string name, object key)
        : base($"Entity \"{name}\" ({key}) is already exists.")
    {
    }
}