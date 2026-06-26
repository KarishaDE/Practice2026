using System;

namespace task04;

public interface ISpaceship
{
    void MoveForward();
    void Rotate(int angle);
    void Fire();
    int Speed { get; }
    int FirePower { get; }
}

public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;

    public void MoveForward() => Console.WriteLine("Крейсер движется вперед со скоростью 50");
    public void Rotate(int angle) => Console.WriteLine($"Крейсер повернулся на {angle} градусов");
    public void Fire() => Console.WriteLine("Крейсер стреляет! Мощность: 100");
}

public class Fighter : ISpaceship
{
    public int Speed => 100;
    public int FirePower => 50;

    public void MoveForward() => Console.WriteLine("Истребитель движется вперед со скоростью 100");
    public void Rotate(int angle) => Console.WriteLine($"Истребитель повернулся на {angle} градусов");
    public void Fire() => Console.WriteLine("Истребитель стреляет! Мощность: 50");
}