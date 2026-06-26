using System;
using System.Linq;

namespace task01;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var filtered = new string(
            input.ToLowerInvariant()
                 .Where(ch => !char.IsWhiteSpace(ch) && !char.IsPunctuation(ch))
                 .ToArray()
        );

        var reversed = new string(filtered.Reverse().ToArray());

        return filtered == reversed;
    }
}