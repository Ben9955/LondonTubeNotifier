namespace LondonTubeNotifier.Core.Exceptions
{
    public class EntityUpdateException : Exception
    {
        public EntityUpdateException() { }

        public EntityUpdateException(string message) : base(message) { }

        public EntityUpdateException(string message, Exception? innerException)
            : base(message, innerException) { }
    }
}



