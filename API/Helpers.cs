using System;
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

    public static string FormatDateTime(this DateTime dateTime)
    {
        string daySuffix = GetDaySuffix(dateTime.Day);
        return dateTime.ToString($"MMMM d'{daySuffix}', yyyy");
    }

    //TO DO
    //add localisation for helpers - need changing for addition of localization
    private static string GetDaySuffix(int day)
    {
        if (day % 10 == 1 && day != 11)
            return "st";
        else if (day % 10 == 2 && day != 12)
            return "nd";
        else if (day % 10 == 3 && day != 13)
            return "rd";
        else
            return "th";
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
