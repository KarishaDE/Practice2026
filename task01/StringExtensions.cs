namespace task01;

/// <summary>
/// Provides helper methods for working with strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Determines whether a string reads the same forwards and backwards,
    /// ignoring letter case, whitespace, and punctuation.
    /// </summary>
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        string filtered = new(
            input.ToLowerInvariant()
                 .Where(character =>
                     !char.IsWhiteSpace(character) &&
                     !char.IsPunctuation(character))
                 .ToArray());

        if (filtered.Length == 0)
        {
            return false;
        }

        string reversed = new(filtered.Reverse().ToArray());

        return filtered.Equals(reversed, StringComparison.Ordinal);
    }
}
