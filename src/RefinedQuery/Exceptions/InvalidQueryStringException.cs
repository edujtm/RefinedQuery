using System;


namespace RefinedQuery.Exceptions
{
    public class InvalidQueryStringException : RefinedQueryException
    {
        public InvalidQueryStringException(string message)
            : base(message) {}

        public InvalidQueryStringException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
