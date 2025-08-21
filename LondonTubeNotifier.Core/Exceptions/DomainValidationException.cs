namespace LondonTubeNotifier.Core.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IEnumerable<string>? Errors { get; }

        public DomainValidationException() { }
        public DomainValidationException(string message, IEnumerable<string>? errors = null)
            : base(message)
        {
            Errors = errors;
        }

        public DomainValidationException(string message, Exception? innerException, IEnumerable<string>? errors = null)
    : base(message, innerException) 
        {
            Errors = errors;
        }
    }
}
