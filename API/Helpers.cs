using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

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

    public static string LocalisedGetDateTimeDurationToCurrentDate(this DateTime startDate, IStringLocalizer localizer)
    {
        DateTime currentDate = DateTime.Now;

        int years = currentDate.Year - startDate.Year;
        int months = currentDate.Month - startDate.Month;

        if (months < 0)
        {
            years--;
            months += 12;
        }

        return $"{years} {localizer["YearsAnd"]} {months} {localizer["Months"]}";
    }


    public static string Pbkdf2HashString(this string password, ref string salt)
    {
        const int SaltSize = 128 / 8;

        if (string.IsNullOrEmpty(salt))
        {
            var newSalt = new byte[SaltSize];
            RandomNumberGenerator.Fill(newSalt);

            salt = Encoding.UTF8.GetString(newSalt);
        }

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var key = KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, 100000, SaltSize);
        return Convert.ToBase64String(key);
    }
}
