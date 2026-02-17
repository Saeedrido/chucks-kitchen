using System.Text.RegularExpressions;

namespace ChuksKitchen.Infrastructure.Helpers;

public static class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Basic phone validation (10-15 digits)
        return Regex.IsMatch(phone, @"^\+?[0-9]{10,15}$");
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return false;

        return true;
    }

    public static bool IsValidAmount(decimal amount)
    {
        return amount > 0;
    }
}
