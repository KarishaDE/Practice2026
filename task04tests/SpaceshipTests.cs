using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_SpeedAndPower_ShouldBeCorrect()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Cruiser_ImplementsISpaceship()
    {
        var cruiser = new Cruiser();
        Assert.IsAssignableFrom<ISpaceship>(cruiser);
    }

    [Fact]
    public void Fighter_SpeedAndPower_ShouldBeCorrect()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(100, fighter.Speed);
        Assert.Equal(50, fighter.FirePower);
    }

    [Fact]
    public void Fighter_ImplementsISpaceship()
    {
        var fighter = new Fighter();
        Assert.IsAssignableFrom<ISpaceship>(fighter);
    }

    [Fact]
    public void Fighter_SpeedGreaterThanCruiser_ShouldBeTrue()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Cruiser_FirePowerGreaterThanFighter_ShouldBeTrue()
    {
        var cruiser = new Cruiser();
        var fighter = new Fighter();
        Assert.True(cruiser.FirePower > fighter.FirePower);
    }

    [Fact]
    public void MoveForward_ShouldExecuteWithoutErrors()
    {
        ISpaceship cruiser = new Cruiser();
        ISpaceship fighter = new Fighter();

        var exception = Record.Exception(() => cruiser.MoveForward());
        Assert.Null(exception);

        exception = Record.Exception(() => fighter.MoveForward());
        Assert.Null(exception);
    }

    [Fact]
    public void Rotate_ShouldExecuteWithoutErrors()
    {
        ISpaceship cruiser = new Cruiser();
        ISpaceship fighter = new Fighter();

        var exception = Record.Exception(() => cruiser.Rotate(45));
        Assert.Null(exception);

        exception = Record.Exception(() => fighter.Rotate(45));
        Assert.Null(exception);
    }

    [Fact]
    public void Fire_ShouldExecuteWithoutErrors()
    {
        ISpaceship cruiser = new Cruiser();
        ISpaceship fighter = new Fighter();

        var exception = Record.Exception(() => cruiser.Fire());
        Assert.Null(exception);

        exception = Record.Exception(() => fighter.Fire());
        Assert.Null(exception);
    }
}