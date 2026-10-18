using CarModel.Core.Enums;
using CarModel.Core.Interfaces;

namespace CarModel.Core.Models
{
    public class ManualTransmissionCar : ICar
    {
        private const double IdleRpm = 800;
        private const double MaxRpm = 7000;
        private const double RedlineRpm = 6500;
        private const double GearRatioReverse = -3.5;
        private readonly Dictionary<Gear, double> _gearRatios = new()
        {
            { Gear.Neutral, 0 },
            { Gear.First, 3.5 },
            { Gear.Second, 2.0 },
            { Gear.Third, 1.5 },
            { Gear.Fourth, 1.0 },
            { Gear.Fifth, 0.8 }
        };
        
        // Свойства
        public CarState State { get; private set; }
        public Gear CurrentGear { get; private set; }
        public ClutchState Clutch { get; private set; }
        public double Speed { get; private set; }
        public double EngineRpm { get; private set; }
        public bool IsEngineRunning { get; private set; }
        
        // Конструктор
        public ManualTransmissionCar()
        {
            State = CarState.Stopped;
            CurrentGear = Gear.Neutral;
            Clutch = ClutchState.Engaged; // Начальное состояние - сцепление выжато
            Speed = 0;
            EngineRpm = 0;
            IsEngineRunning = false;
        }
        
        // Запуск двигателя
        public bool StartEngine()
        {
            if (IsEngineRunning)
                return false;
            
            if (CurrentGear != Gear.Neutral)
                return false;
            
            IsEngineRunning = true;
            State = CarState.Running;
            EngineRpm = IdleRpm;
            return true;
        }
        
        // Остановка двигателя
        public bool StopEngine()
        {
            if (!IsEngineRunning)
                return false;
            
            if (State == CarState.Moving)
                return false;
            
            if (CurrentGear != Gear.Neutral)
                return false;
            
            IsEngineRunning = false;
            State = CarState.Stopped;
            EngineRpm = 0;
            return true;
        }
        
        // Выжать сцепление
        public bool PressClutch()
        {
            if (!IsEngineRunning)
                return false;
            
            Clutch = ClutchState.Engaged;
            return true;
        }
        
        // Отпустить сцепление
        public bool ReleaseClutch()
        {
            if (!IsEngineRunning)
                return false;
            
            // Нельзя отпустить сцепление на неподходящей передаче при скорости
            if (CurrentGear != Gear.Neutral && Speed > 0)
            {
                double minSpeed = GetMinSpeedForGear(CurrentGear);
                double maxSpeed = GetMaxSpeedForGear(CurrentGear);
                
                if (Speed < minSpeed || Speed > maxSpeed)
                    return false;
            }
            
            Clutch = ClutchState.Disengaged;
            UpdateEngineRpm();
            return true;
        }
        
        // Переключить передачу
        public bool ShiftGear(Gear gear)
        {
            if (!IsEngineRunning)
                return false;
            
            if (Clutch != ClutchState.Engaged)
                return false;
            
            // Проверка на допустимость переключения
            if (!IsValidGearShift(gear))
                return false;
            
            CurrentGear = gear;
            
            // Если отпустили сцепление, обновляем RPM
            if (Clutch == ClutchState.Disengaged)
                UpdateEngineRpm();
            
            return true;
        }
        
        // Нажать педаль газа
        public bool Accelerate(double pressure)
        {
            if (!IsEngineRunning)
                return false;
            
            pressure = Math.Clamp(pressure, 0, 1);
            
            if (CurrentGear == Gear.Neutral)
            {
                // На нейтрали двигатель просто ревет
                EngineRpm = Math.Min(MaxRpm, IdleRpm + (MaxRpm - IdleRpm) * pressure);
                return true;
            }
            
            if (Clutch == ClutchState.Engaged)
            {
                // При выжатом сцеплении двигатель просто ревет
                EngineRpm = Math.Min(MaxRpm, IdleRpm + (MaxRpm - IdleRpm) * pressure);
                return true;
            }
            
            // Рассчитываем ускорение
            double acceleration = pressure * GetGearRatio() * 0.1;
            Speed += acceleration;
            
            // Ограничиваем скорость
            Speed = Math.Min(Speed, GetMaxSpeedForGear(CurrentGear));
            
            // Обновляем состояние
            State = Speed > 0 ? CarState.Moving : CarState.Running;
            
            // Обновляем RPM двигателя
            UpdateEngineRpm();
            
            // Проверка на превышение RPM
            if (EngineRpm > RedlineRpm)
            {
                // Симуляция повреждения двигателя при превышении RPM
                EngineRpm = RedlineRpm;
            }
            
            return true;
        }
        
        // Тормозить
        public bool Brake(double pressure)
        {
            if (!IsEngineRunning)
                return false;
            
            pressure = Math.Clamp(pressure, 0, 1);
            
            // Рассчитываем замедление
            double deceleration = pressure * 5;
            Speed = Math.Max(0, Speed - deceleration);
            
            // Обновляем состояние
            if (Speed == 0)
                State = CarState.Running;
            
            // Обновляем RPM двигателя
            UpdateEngineRpm();
            
            return true;
        }
        
        // Получить статус автомобиля
        public string GetStatus()
        {
            return $"Состояние: {State}\n" +
                   $"Двигатель: {(IsEngineRunning ? "Запущен" : "Заглушен")}\n" +
                   $"Скорость: {Speed:F1} км/ч\n" +
                   $"Обороты: {EngineRpm:F0} RPM\n" +
                   $"Передача: {GetGearName(CurrentGear)}\n" +
                   $"Сцепление: {(Clutch == ClutchState.Engaged ? "Выжато" : "Отпущено")}";
        }
        
        // Вспомогательные методы
        private void UpdateEngineRpm()
        {
            if (CurrentGear == Gear.Neutral || Clutch == ClutchState.Engaged)
            {
                // На нейтрали или при выжатом сцеплении RPM на холостом ходу
                EngineRpm = IsEngineRunning ? IdleRpm : 0;
            }
            else
            {
                // Рассчитываем RPM на основе скорости и передачи
                double gearRatio = GetGearRatio();
                EngineRpm = IdleRpm + (Speed * 100 * Math.Abs(gearRatio));
                EngineRpm = Math.Clamp(EngineRpm, IdleRpm, MaxRpm);
            }
        }
        
        private double GetGearRatio()
        {
            if (CurrentGear == Gear.Reverse)
                return GearRatioReverse;
            
            return _gearRatios[CurrentGear];
        }
        
        private bool IsValidGearShift(Gear newGear)
        {
            // Нельзя переключиться на ту же передачу
            if (CurrentGear == newGear)
                return false;
            
            // Особые проверки для задней передачи
            if (newGear == Gear.Reverse)
            {
                // Можно включить заднюю только при полной остановке
                return Speed == 0;
            }
            
            if (CurrentGear == Gear.Reverse)
            {
                // Можно выйти из задней только при полной остановке
                return Speed == 0;
            }
            
            return true;
        }
        
        private double GetMinSpeedForGear(Gear gear)
        {
            return gear switch
            {
                Gear.First => 0,
                Gear.Second => 5,
                Gear.Third => 15,
                Gear.Fourth => 25,
                Gear.Fifth => 40,
                _ => 0
            };
        }
        
        private double GetMaxSpeedForGear(Gear gear)
        {
            return gear switch
            {
                Gear.First => 30,
                Gear.Second => 60,
                Gear.Third => 90,
                Gear.Fourth => 120,
                Gear.Fifth => 180,
                Gear.Reverse => 20,
                _ => 0
            };
        }
        
        private string GetGearName(Gear gear)
        {
            return gear switch
            {
                Gear.Reverse => "R",
                Gear.Neutral => "N",
                _ => ((int)gear).ToString()
            };
        }
    }
}