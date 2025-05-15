using System;

namespace TheSSS.DicomViewer.Common.Exceptions
{
    [Serializable]
    public class CalculationException : Exception
    {
        public CalculationException() { }
        public CalculationException(string message) : base(message) { }
        public CalculationException(string message, Exception inner) : base(message, inner) { }
        protected CalculationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}