using System;

namespace RefinedQuery.Ordering
{
    public class InvalidQueryFieldException : Exception
    {
        public InvalidQueryFieldException() 
            : base() {}

        public InvalidQueryFieldException(string message)
            : base(message) {}

        public InvalidQueryFieldException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}