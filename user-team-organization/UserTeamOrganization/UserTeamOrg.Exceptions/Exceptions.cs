using System;
namespace UserTeamOrg.Exceptions
{
    public class NoTeamIdException : Exception
    {
        public override string Message { get; }

        public NoTeamIdException(string message = "No team Id provided")
        {
            Message = message;
        }
    }

    public class UserExistsException : Exception
    {
        public override string Message { get; }

        public UserExistsException(string message = "A user already exists with this email.")
        {
            Message = message;
        }
    }

    public class NotFoundException : Exception
    {
        public override string Message { get; }

        public NotFoundException(string message = "Requested resource could not be found.")
        {
            Message = message;
        }
    }


}

