using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();

        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();

        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();

        Assert.Equal(100, fighter.Speed);
        Assert.Equal(50, fighter.FirePower);
    }

    [Fact]
    public void Cruiser_ShouldHaveMoreFirePowerThanFighter()
    {
        var cruiser = new Cruiser();
        var fighter = new Fighter();

        Assert.True(cruiser.FirePower > fighter.FirePower);
    }

    [Fact]
    public void MoveForward_ShouldIncreaseDistanceBySpeed()
    {
        var cruiser = new Cruiser();

        cruiser.MoveForward();

        Assert.Equal(50, cruiser.DistanceTraveled);
    }

    [Fact]
    public void Rotate_ShouldChangeRotationAngle()
    {
        var fighter = new Fighter();

        fighter.Rotate(90);

        Assert.Equal(90, fighter.RotationAngle);
    }

    [Fact]
    public void Fire_ShouldIncreaseFiredRocketsCount()
    {
        var cruiser = new Cruiser();

        cruiser.Fire();
        cruiser.Fire();

        Assert.Equal(2, cruiser.FiredRockets);
    }

    [Fact]
    public void CruiserAndFighter_ShouldImplementISpaceship()
    {
        Assert.IsAssignableFrom<ISpaceship>(new Cruiser());
        Assert.IsAssignableFrom<ISpaceship>(new Fighter());
    }
}
