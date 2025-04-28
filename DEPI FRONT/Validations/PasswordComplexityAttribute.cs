using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ecommerce.Frontend.Validations
{
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult("Password is required.");

            if (password.Length < 6)
                return new ValidationResult("Password must be at least 6 characters.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return new ValidationResult("Password must have at least one uppercase letter.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return new ValidationResult("Password must have at least one lowercase letter.");

            if (!Regex.IsMatch(password, @"[\W_]"))
                return new ValidationResult("Password must have at least one symbol.");

            return ValidationResult.Success!;
        }
    }
}
