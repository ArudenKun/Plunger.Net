using System.Runtime.Serialization;
using Plunger.Commons.Formatters;

namespace Plunger.Commons.Exceptions;

[Serializable]
public class NeatException : Exception
{
    public NeatException() : base() { }
    public NeatException(string message) : base(message) { }
    public NeatException(string message, Exception inner) : base(message, inner) { }

    protected NeatException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    public override string ToString()
    {
        return "Response is Null" ?? base.ToString();
    }
}