using System;

namespace RefinedQuery.Exceptions
{
    public class RefinedQueryException : Exception
    {
        public RefinedQueryException(string message)
            : base(message) {}

        public RefinedQueryException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}