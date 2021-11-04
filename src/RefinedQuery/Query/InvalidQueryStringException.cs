using System;


namespace RefinedQuery.Query
{
    public class InvalidQueryStringException : Exception
    {
        public InvalidQueryStringException() 
            : base() {}

        public InvalidQueryStringException(string message)
            : base(message) {}

        public InvalidQueryStringException(string message, Exception innerException)
            : base(message, innerException) {}
    }
}
