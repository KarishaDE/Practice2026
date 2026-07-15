namespace task04;

public class Fighter : ISpaceship
{
    public int Speed => 100;

    public int FirePower => 50;

    public int DistanceTraveled { get; private set; }

    public int RotationAngle { get; private set; }

    public int FiredRockets { get; private set; }

    public void MoveForward() => DistanceTraveled += Speed;

    public void Rotate(int angle) => RotationAngle += angle;

    public void Fire() => FiredRockets++;
}
