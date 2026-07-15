using task01;

namespace task01tests;

public class StringExtensionsTests
{
    [Fact]
    public void IsPalindrome_RussianPhrase_ReturnsTrue()
    {
        string input = "А роза упала на лапу Азора";

        Assert.True(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_NotPalindrome_ReturnsFalse()
    {
        string input = "Hello, world!";

        Assert.False(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_EmptyString_ReturnsFalse()
    {
        string input = string.Empty;

        Assert.False(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_WithPunctuation_IgnoresPunctuation()
    {
        string input = "Was it a car or a cat I saw?";

        Assert.True(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_DifferentLetterCase_IgnoresCase()
    {
        string input = "LeVEl";

        Assert.True(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_OnlyWhitespace_ReturnsFalse()
    {
        string input = "   \t\r\n";

        Assert.False(input.IsPalindrome());
    }

    [Fact]
    public void IsPalindrome_OnlyPunctuation_ReturnsFalse()
    {
        string input = "...!";

        Assert.False(input.IsPalindrome());
    }
}
