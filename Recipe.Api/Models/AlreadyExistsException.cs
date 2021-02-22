using System;

namespace Recipe.Api.Models
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
}