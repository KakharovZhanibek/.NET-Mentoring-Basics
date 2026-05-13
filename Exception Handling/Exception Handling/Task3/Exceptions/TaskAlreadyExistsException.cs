using System;

namespace Task3.Exceptions
{
    public class TaskAlreadyExistsException : Exception
    {
        public TaskAlreadyExistsException() : base("The task already exists")
        {
        }

        public TaskAlreadyExistsException(string message) : base(message)
        {
        }

        public TaskAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
