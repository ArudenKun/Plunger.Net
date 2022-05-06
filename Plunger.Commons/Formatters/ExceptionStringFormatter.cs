using System.Diagnostics;
using System.Reflection;
using System.Text;
using Plunger.Commons.Enums;
using Plunger.Commons.Extensions;
using Plunger.Commons.QuickLinq;

namespace Plunger.Commons.Formatters;

public static class ExceptionStringFormatter
{
    public static string Prettify(this Exception exception, ExceptionOrder order = ExceptionOrder.Descending, int indentWidth = 4)
    {
        return order switch
        {
            ExceptionOrder.Ascending => string.Join(Environment.NewLine, exception.PrettifyCore(0, indentWidth).Reverse()),
            ExceptionOrder.Descending => string.Join(Environment.NewLine, exception.PrettifyCore(0, indentWidth)),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, $"unsupported order {order}"),
        };
    }

    private static IEnumerable<string> PrettifyCore(this Exception exception, int indent, int indentWidth)
    {
        var builder = new StringBuilder();

        var makeIndent = new Func<int, string>((depth) => new string(' ', indentWidth * (depth + indent)));

        builder.AppendLine($"{makeIndent(1)}{exception.GetType().Name}: \"{exception.Message}\"");

        if (exception is AggregateException exception2)
        {
            builder.AppendLine($"{makeIndent(2)}InnerExceptions: \"{exception2.InnerExceptions.Count}\"");
        }

        foreach (var property in exception.GetPropertiesExcept<Exception>())
        {
            builder.AppendLine($"{makeIndent(1)}{property.Name}: \"{property.Value}\"");
        }

        foreach (var property in exception.GetData())
        {
            builder.AppendLine($"{makeIndent(1)}Data[{property.Key}]: \"{property.Value}\"");
        }

        builder.AppendLine($"{makeIndent(2)}StackTrace:");

        foreach (var stackTrace in exception.GetStackTrace() ?? Enumerable.Empty<StackFrame>())
        {
            builder.AppendLine($"{makeIndent(3)}{stackTrace.Caller} in \"{stackTrace.FileName}\" Ln {stackTrace.LineNumber}");
        }

        yield return builder.ToString();

        if (exception is AggregateException exception1)
        {
            foreach (var subStrings in exception1.InnerExceptions.Select(ex => ex.PrettifyCore(indent + 1, indentWidth)))
            {
                foreach (var subString in subStrings)
                {
                    yield return subString;
                }
            }
        }
        else if (exception.InnerException != null)
        {
            foreach (var subString in exception.InnerException.PrettifyCore(indent + 1, indentWidth))
            {
                yield return subString;
            }
        }
    }


    public static IEnumerable<Property> GetPropertiesExcept<TException>(this Exception exception) where TException : Exception
    {
        var propertyFlags = BindingFlags.Instance | BindingFlags.Public;

        var properties = exception.GetType()
            .GetProperties(propertyFlags)
            .Except(typeof(TException).GetProperties(propertyFlags))
            .Select(p => new Property
            {
                Name = p.Name,
                Value = p.GetValue(exception) ?? new object()
            })
            .Where(p => p.Value != null && !string.IsNullOrWhiteSpace(p.Value as string));

        return properties;
    }

    private static IEnumerable<Data> GetData(this Exception exception)
    {
        foreach (var key in exception.Data.Keys)
        {
            yield return new()
            {
                Key = key,
                Value = exception.Data[key] ?? new object()
            };
        }
    }

    private static IEnumerable<StackFrame> GetStackTrace(this Exception exception)
    {
        var stackTrace = new StackTrace(exception, true);
        var stackFrames = stackTrace.GetFrames();
        var result = stackFrames.SelectQ(sf => new StackFrame
        {
            Caller = (sf.GetMethod() as MethodInfo)?.ToShortString() ?? "",
            FileName = Path.GetFileName(sf.GetFileName()) ?? "UNKNOWN",
            LineNumber = sf.GetFileLineNumber()
        });

        return result!;
    }
}

public class Data
{
    public object? Key { get; init; }
    public object? Value { get; init; }
}

public class Property
{
    public string? Name { get; init; }
    public object? Value { get; init; }
}

public class ExceptionInfo
{
    public Type? ExceptionType { get; init; }
    public string? ExceptionMessage { get; init; }
    public Dictionary<string, object>? CustomProperties { get; init; }
    public StackFrame[]? StackTrace { get; init; }
}

public class StackFrame
{
    public string? Caller { get; init; }
    public string? FileName { get; init; }
    public int LineNumber { get; init; }
}