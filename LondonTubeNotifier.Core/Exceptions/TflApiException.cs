using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LondonTubeNotifier.Core.Exceptions
{
    public class TflApiException : Exception
    {
        public TflApiException() { }
        public TflApiException(string message) : base(message) { }
        public TflApiException(string message, Exception? innerException)
    : base(message, innerException) { }
    }

}
