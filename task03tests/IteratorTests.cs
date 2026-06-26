using Xunit;
using System.Collections.Generic;
using System.Linq;
using task03;

namespace task03tests;

public class IteratorTests
{
    [Fact]
    public void AddElements_ShouldBeIterable()
    {
        var collection = new CustomCollection<string>();
        collection.AddElement("first");
        collection.AddElement("second");

        var result = new List<string>();
        foreach (var item in collection)
        {
            result.Add(item);
        }

        Assert.Equal(new[] { "first", "second" }, result);
    }

    [Fact]
    public void GetReverse_ShouldReturnReversedOrder()
    {
        var collection = new CustomCollection<int>();
        collection.AddElement(1);
        collection.AddElement(2);
        collection.AddElement(3);

        var result = collection.GetReverse().ToList();
        Assert.Equal(new[] { 3, 2, 1 }, result);
    }

    [Fact]
    public void CreateNumberRange_ShouldGenerateCorrectNumbers()
    {
        var sequence = CustomCollection<int>.CreateNumberRange(5, 3).ToList();
        Assert.Equal(new[] { 5, 6, 7 }, sequence);
    }

    [Fact]
    public void ApplyFilterAndOrder_ShouldFilterAndSort()
    {
        var collection = new CustomCollection<int>();
        collection.AddElement(3);
        collection.AddElement(1);
        collection.AddElement(2);

        var result = collection.ApplyFilterAndOrder(x => x > 1, x => x).ToList();
        Assert.Equal(new[] { 2, 3 }, result);
    }

    [Fact]
    public void DeleteElement_ShouldRemoveItem()
    {
        var collection = new CustomCollection<int>();
        collection.AddElement(1);
        collection.AddElement(2);
        collection.AddElement(3);

        bool isDeleted = collection.DeleteElement(2);

        Assert.True(isDeleted);
        Assert.Equal(2, collection.ElementsCount);

        var remaining = new List<int>();
        foreach (var item in collection)
        {
            remaining.Add(item);
        }

        Assert.DoesNotContain(2, remaining);
        Assert.Equal(new[] { 1, 3 }, remaining);
    }

    [Fact]
    public void DeleteElement_ShouldReturnFalseIfNotFound()
    {
        var collection = new CustomCollection<int>();
        collection.AddElement(1);
        collection.AddElement(2);

        bool result = collection.DeleteElement(99);

        Assert.False(result);
        Assert.Equal(2, collection.ElementsCount);
    }
}