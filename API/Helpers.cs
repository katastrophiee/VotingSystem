using System.ComponentModel.DataAnnotations;
namespace VotingSystem;

public static class Helpers
{
    public static string EnumDisplayName(this Enum enumObj)
    {
        var displayName = enumObj.ToString();

        var fieldInfo = enumObj.GetType().GetField(displayName);
        if (fieldInfo == null)
            return displayName;

        var attribArray = fieldInfo.GetCustomAttributes(false);
        if (attribArray.Length == 0)
            return displayName;

        if (attribArray[0] is not DisplayAttribute attrib)
            return displayName;

        return attrib.Name;
    }
}
