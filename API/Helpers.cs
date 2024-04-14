using Microsoft.Extensions.Localization;
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

    public static string LocalisedEnumDisplayName(this Enum enumObj, IStringLocalizer localizer)
    {
        var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
        if (fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute attrib)
        {
            var localizedDisplayName = localizer[attrib.Name ?? ""];
            return !string.IsNullOrEmpty(localizedDisplayName) ? localizedDisplayName : enumObj.ToString();
        }
        else
        {
            return enumObj.ToString();
        }
    }

    public static string FormatDateTime(this DateTime dateTime)
    {
        return dateTime.ToString($"MMMM d, yyyy");
    }

    public static bool? StringToNullableBool(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        if (bool.TryParse(str, out bool result))
            return result;

        return null;
    }

    public static string GetDateTimeDurationToCurrentDate(this DateTime startDate)
    {
        DateTime currentDate = DateTime.Now;

        int years = currentDate.Year - startDate.Year;
        int months = currentDate.Month - startDate.Month;

        if (months < 0)
        {
            years--;
            months += 12;
        }

        return $"{years} years and {months} months";
    }
}
