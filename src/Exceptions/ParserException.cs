using System;

namespace Microsoft.AppInnovation.Budgets.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException() { }
        public ParserException(string message) : base(message) { }
    }
}