using System.ComponentModel.DataAnnotations;

namespace VotingSystem;

public static class Helpers
{
    public static string EnumDisplayName(this Enum enumObj)
    {
        var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
        var attrib = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
        return attrib?.Name ?? enumObj.ToString();
    }

    public static string EnumDescription(this Enum enumObj)
    {
        var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
        var attrib = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
        return attrib?.Description ?? enumObj.ToString();
    }
}
