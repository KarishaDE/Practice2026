using Xunit;
using task01;

namespace task01tests;

public class StringExtensionsTests
{
    [Fact]
    public void CheckPalindrome_ShouldReturnTrueForValidPhrase()
    {
        string text = "Казак";
        bool result = text.IsPalindrome();
        Assert.True(result);
    }

    [Fact]
    public void CheckPalindrome_ShouldReturnTrueForComplexPhrase()
    {
        string text = "Sum summus mus";
        bool result = text.IsPalindrome();
        Assert.True(result);
    }

    [Fact]
    public void CheckPalindrome_ShouldReturnFalseForInvalidPhrase()
    {
        string text = "qwerty";
        bool result = text.IsPalindrome();
        Assert.False(result);
    }

    [Fact]
    public void CheckPalindrome_ShouldReturnFalseForEmptyString()
    {
        string text = "";
        bool result = text.IsPalindrome();
        Assert.False(result);
    }

    [Fact]
    public void CheckPalindrome_ShouldReturnTrueWithPunctuation()
    {
        string text = "Madam, I'm Adam";
        bool result = text.IsPalindrome();
        Assert.True(result);
    }

    [Fact]
    public void CheckPalindrome_ShouldReturnTrueForNumericPalindrome()
    {
        string text = "123454321";
        bool result = text.IsPalindrome();
        Assert.True(result);
    }

    [Fact]
    public void CheckPalindrome_ShouldReturnTrueForSingleCharacter()
    {
        string text = "a";
        bool result = text.IsPalindrome();
        Assert.True(result);
    }
}