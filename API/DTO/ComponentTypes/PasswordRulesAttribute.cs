using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VotingSystem.API.DTO.ComponentTypes;

public class PasswordRulesAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || value.ToString() == "")
        {
            return ValidationResult.Success!;
        }

        var password = value.ToString();

        if (!Regex.IsMatch(password ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$"))
        {
            return new ValidationResult("Password must be between 8 and 15 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.");
        }

        return ValidationResult.Success!;
    }
}