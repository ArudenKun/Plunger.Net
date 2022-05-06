using System.Reflection;
using Plunger.Commons.Attributes;
using Plunger.Commons.QuickLinq;

namespace Plunger.Commons.Extensions;

/// <summary>
/// Provides a set of extensions for working with enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
	/// Gets the string or T value from an enum if possible, otherwise just returns the name.
	/// Use the [ValueAttribute] on enum fields if you wish to take advantage of this extension
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	public static string? GetValue(this Enum input)
    {
        var memberInfo = input.GetType().GetMember(input.ToString()).FirstOrDefaultQ();
        if (memberInfo is null)
        {
            return null;
        }

        var attribute = memberInfo.GetCustomAttributes<ValueAttribute>(false).FirstOrDefault();
        if (attribute is null)
        {
            return null;
        }

        return attribute.Value;
    }
}
