using System.Globalization;
using System.Reflection;
using System.Text;
using Plunger.Commons.QuickLinq;

namespace Plunger.Commons.Formatters;

public static class MethodSignatureFormatters
{
    /// <summary>
    /// Generic formatter to print a method signature in string format 
    /// </summary>
    /// <example>
    ///	public int Sum(int x, int y) 
    /// </example>
    /// <param name="method">The method that is to be formatted</param>
    /// <returns>A pretty string of the method signature</returns>
    public static string Format(this MethodInfo method)
    {
        if (method is MethodInfo methodInfo)
        {
            var accessor = "private";

            if (methodInfo.IsPublic)
            {
                accessor = "public";
            }

            return new StringBuilder()
                .Append(accessor).Append(' ')
                .Append(methodInfo.IsStatic ? "static " : "")
                .Append(methodInfo.ReturnType.FullName)
                .Append(' ').Append(methodInfo.DeclaringType?.FullName)
                .Append('.').Append(methodInfo.Name)
                .Append('(')
                .Append(string.Join(", ", methodInfo.GetParameters().SelectQ(p => p.ParameterType.FullName + " " + p.Name)))
                .Append(')')
                .ToString();
        }

        return method.Name;
    }
}
