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

    public static string EnumDescription(this Enum enumObj)
    {
        var description = enumObj.ToString();

        var fieldInfo = enumObj.GetType().GetField(description);
        if (fieldInfo == null)
            return description;

        var attribArray = fieldInfo.GetCustomAttributes(false);
        if (attribArray.Length == 0)
            return description;

        if (attribArray[0] is not DisplayAttribute attrib)
            return description;

        return attrib.Description;
    }
}
