using CarModel.Core.Enums;
using CarModel.Core.Models;

namespace CarModel.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            var car = new ManualTransmissionCar();
            bool running = true;
            
            Console.WriteLine("=== СИМУЛЯТОР АВТОМОБИЛЯ С МКПП ===");
            Console.WriteLine("\nУправление:");
            Console.WriteLine("  [S] - Запустить двигатель");
            Console.WriteLine("  [E] - Заглушить двигатель");
            Console.WriteLine("  [C] - Выжать сцепление");
            Console.WriteLine("  [V] - Отпустить сцепление");
            Console.WriteLine("  [1-5] - Передачи 1-5");
            Console.WriteLine("  [R] - Задняя передача");
            Console.WriteLine("  [N] - Нейтральная передача");
            Console.WriteLine("  [A] - Газ (ускорение)");
            Console.WriteLine("  [B] - Тормоз");
            Console.WriteLine("  [I] - Информация о состоянии");
            Console.WriteLine("  [Q] - Выход");
            
            while (running)
            {
                Console.WriteLine("\n" + new string('=', 50));
                DisplayCarStatus(car);
                Console.Write("\nВведите команду: ");
                
                var key = Console.ReadKey().KeyChar.ToString().ToUpper();
                Console.WriteLine();
                
                switch (key)
                {
                    case "S":
                        HandleStartEngine(car);
                        break;
                        
                    case "E":
                        HandleStopEngine(car);
                        break;
                        
                    case "C":
                        HandlePressClutch(car);
                        break;
                        
                    case "V":
                        HandleReleaseClutch(car);
                        break;
                        
                    case "1":
                        HandleShiftGear(car, Gear.First);
                        break;
                        
                    case "2":
                        HandleShiftGear(car, Gear.Second);
                        break;
                        
                    case "3":
                        HandleShiftGear(car, Gear.Third);
                        break;
                        
                    case "4":
                        HandleShiftGear(car, Gear.Fourth);
                        break;
                        
                    case "5":
                        HandleShiftGear(car, Gear.Fifth);
                        break;
                        
                    case "R":
                        HandleShiftGear(car, Gear.Reverse);
                        break;
                        
                    case "N":
                        HandleShiftGear(car, Gear.Neutral);
                        break;
                        
                    case "A":
                        HandleAccelerate(car);
                        break;
                        
                    case "B":
                        HandleBrake(car);
                        break;
                        
                    case "I":
                        DisplayDetailedStatus(car);
                        break;
                        
                    case "Q":
                        running = false;
                        Console.WriteLine("Выход из симулятора...");
                        break;
                        
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
                
                // Небольшая задержка для реалистичности
                Thread.Sleep(100);
            }
        }
        
        static void DisplayCarStatus(ManualTransmissionCar car)
        {
            Console.WriteLine($"Двигатель: {(car.IsEngineRunning ? "✅ ЗАПУЩЕН" : "⭕ ЗАГЛУШЕН")}");
            Console.WriteLine($"Скорость: {car.Speed:F1} км/ч");
            Console.WriteLine($"Обороты: {car.EngineRpm:F0} RPM");
            
            string gearDisplay = car.CurrentGear switch
            {
                Gear.Reverse => "R",
                Gear.Neutral => "N",
                _ => ((int)car.CurrentGear).ToString()
            };
            
            Console.WriteLine($"Передача: [{gearDisplay}]");
            Console.WriteLine($"Сцепление: {(car.Clutch == ClutchState.Engaged ? "ВЫЖАТО" : "ОТПУЩЕНО")}");
            Console.WriteLine($"Состояние: {GetStateDisplay(car.State)}");
        }
        
        static string GetStateDisplay(CarState state)
        {
            return state switch
            {
                CarState.Stopped => "⏹️ ОСТАНОВЛЕН",
                CarState.Running => "▶️ РАБОТАЕТ",
                CarState.Moving => "🚗 ДВИЖЕТСЯ",
                _ => "НЕИЗВЕСТНО"
            };
        }
        
        static void HandleStartEngine(ManualTransmissionCar car)
        {
            if (car.StartEngine())
            {
                Console.WriteLine("✅ Двигатель запущен");
            }
            else
            {
                Console.WriteLine("❌ Не удалось запустить двигатель. Проверьте, что передача в нейтральном положении.");
            }
        }
        
        static void HandleStopEngine(ManualTransmissionCar car)
        {
            if (car.StopEngine())
            {
                Console.WriteLine("✅ Двигатель заглушен");
            }
            else
            {
                Console.WriteLine("❌ Не удалось заглушить двигатель. Убедитесь, что автомобиль не движется и передача в нейтральном положении.");
            }
        }
        
        static void HandlePressClutch(ManualTransmissionCar car)
        {
            if (car.PressClutch())
            {
                Console.WriteLine("✅ Сцепление выжато");
            }
            else
            {
                Console.WriteLine("❌ Не удалось выжать сцепление. Запустите двигатель.");
            }
        }
        
        static void HandleReleaseClutch(ManualTransmissionCar car)
        {
            if (car.ReleaseClutch())
            {
                Console.WriteLine("✅ Сцепление отпущено");
            }
            else
            {
                Console.WriteLine("❌ Не удалось отпустить сцепление. Проверьте скорость и передачу.");
            }
        }
        
        static void HandleShiftGear(ManualTransmissionCar car, Gear gear)
        {
            string gearName = gear switch
            {
                Gear.Reverse => "заднюю",
                Gear.Neutral => "нейтральную",
                Gear.First => "первую",
                Gear.Second => "вторую",
                Gear.Third => "третью",
                Gear.Fourth => "четвертую",
                Gear.Fifth => "пятую",
                _ => "неизвестную"
            };
            
            if (car.ShiftGear(gear))
            {
                Console.WriteLine($"✅ Переключились на {gearName} передачу");
            }
            else
            {
                Console.WriteLine($"❌ Не удалось переключиться на {gearName} передачу. Выжмите сцепление и проверьте условия.");
            }
        }
        
        static void HandleAccelerate(ManualTransmissionCar car)
        {
            Console.Write("Введите силу нажатия на газ (0.0-1.0): ");
            if (double.TryParse(Console.ReadLine(), out double pressure))
            {
                if (car.Accelerate(pressure))
                {
                    Console.WriteLine("✅ Газ нажат");
                }
                else
                {
                    Console.WriteLine("❌ Не удалось ускориться");
                }
            }
            else
            {
                Console.WriteLine("❌ Неверный ввод");
            }
        }
        
        static void HandleBrake(ManualTransmissionCar car)
        {
            Console.Write("Введите силу нажатия на тормоз (0.0-1.0): ");
            if (double.TryParse(Console.ReadLine(), out double pressure))
            {
                if (car.Brake(pressure))
                {
                    Console.WriteLine("✅ Торможение");
                }
                else
                {
                    Console.WriteLine("❌ Не удалось затормозить");
                }
            }
            else
            {
                Console.WriteLine("❌ Неверный ввод");
            }
        }
        
        static void DisplayDetailedStatus(ManualTransmissionCar car)
        {
            Console.WriteLine("\n=== ДЕТАЛЬНАЯ ИНФОРМАЦИЯ ===");
            Console.WriteLine(car.GetStatus());
        }
    }
}