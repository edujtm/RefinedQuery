using System;

namespace RefinedQuery.Exceptions
{
    public class InvalidQueryFieldException : RefinedQueryException
    {
        public InvalidQueryFieldException(string message)
            : base(message) {}

        public InvalidQueryFieldException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
