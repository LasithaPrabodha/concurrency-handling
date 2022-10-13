using System.Runtime.Serialization;

namespace UsersWebApi
{
    [Serializable]
    internal class PreconditionFailedException : Exception
    {
        public PreconditionFailedException()
        {
        }

        public PreconditionFailedException(string? message) : base(message)
        {
        }

        public PreconditionFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PreconditionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}