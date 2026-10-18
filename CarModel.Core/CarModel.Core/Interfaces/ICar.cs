using CarModel.Core.Enums;

namespace CarModel.Core.Interfaces
{
    public interface ICar
    {
        // Свойства
        CarState State { get; }
        Gear CurrentGear { get; }
        ClutchState Clutch { get; }
        double Speed { get; }
        double EngineRpm { get; }
        bool IsEngineRunning { get; }
        
        // Методы управления
        bool StartEngine();
        bool StopEngine();
        bool PressClutch();
        bool ReleaseClutch();
        bool ShiftGear(Gear gear);
        bool Accelerate(double pressure);
        bool Brake(double pressure);
        
        // Методы состояния
        string GetStatus();
    }
}