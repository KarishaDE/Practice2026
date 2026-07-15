namespace task04;

public class Cruiser : ISpaceship
{
    public int Speed => 50;

    public int FirePower => 100;

    public int DistanceTraveled { get; private set; }

    public int RotationAngle { get; private set; }

    public int FiredRockets { get; private set; }

    public void MoveForward() => DistanceTraveled += Speed;

    public void Rotate(int angle) => RotationAngle += angle;

    public void Fire() => FiredRockets++;
}
