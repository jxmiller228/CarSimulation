using CarModel.Core.Enums;
using CarModel.Core.Models;
using Xunit;

namespace CarModel.Tests
{
    public class ManualTransmissionCarTests
    {
        [Fact]
        public void CreateCar_InitialStateIsCorrect()
        {
            
            var car = new ManualTransmissionCar();
            
            
            Assert.Equal(CarState.Stopped, car.State);
            Assert.Equal(Gear.Neutral, car.CurrentGear);
            Assert.Equal(ClutchState.Engaged, car.Clutch);
            Assert.Equal(0, car.Speed);
            Assert.Equal(0, car.EngineRpm);
            Assert.False(car.IsEngineRunning);
        }
        
        [Fact]
        public void StartEngine_SuccessWhenInNeutral()
        {
            
            var car = new ManualTransmissionCar();
            
            
            var result = car.StartEngine();
            
            
            Assert.True(result);
            Assert.True(car.IsEngineRunning);
            Assert.Equal(CarState.Running, car.State);
            Assert.True(car.EngineRpm > 0);
        }
        
        [Fact]
        public void StartEngine_FailWhenAlreadyRunning()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            
            
            var result = car.StartEngine();
            
            
            Assert.False(result);
        }
        
        [Fact]
        public void StopEngine_SuccessWhenNotMovingAndInNeutral()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            
            
            var result = car.StopEngine();
            
            
            Assert.True(result);
            Assert.False(car.IsEngineRunning);
            Assert.Equal(CarState.Stopped, car.State);
            Assert.Equal(0, car.EngineRpm);
        }
        
        [Fact]
        public void ShiftGear_SuccessWithClutchEngaged()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            
            
            var result = car.ShiftGear(Gear.First);
            
            
            Assert.True(result);
            Assert.Equal(Gear.First, car.CurrentGear);
        }
        
        [Fact]
        public void ShiftGear_FailWithoutClutchEngaged()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            car.ReleaseClutch();
            
            
            var result = car.ShiftGear(Gear.First);
            
            
            Assert.False(result);
            Assert.Equal(Gear.Neutral, car.CurrentGear);
        }
        
        [Fact]
        public void Accelerate_IncreasesSpeedWhenInGear()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            car.ShiftGear(Gear.First);
            car.ReleaseClutch();
            
            
            var result = car.Accelerate(0.5);
            
            
            Assert.True(result);
            Assert.True(car.Speed > 0);
            Assert.True(car.EngineRpm > 800);
            Assert.Equal(CarState.Moving, car.State);
        }
        
        [Fact]
        public void Accelerate_IncreasesRpmWhenInNeutral()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            
            
            var result = car.Accelerate(0.7);
            
            
            Assert.True(result);
            Assert.Equal(0, car.Speed);
            Assert.True(car.EngineRpm > 800);
            Assert.Equal(CarState.Running, car.State);
        }
        
        [Fact]
        public void Brake_DecreasesSpeed()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            car.ShiftGear(Gear.First);
            car.ReleaseClutch();
            car.Accelerate(0.5);
            var initialSpeed = car.Speed;
            
            
            var result = car.Brake(0.3);
            
            
            Assert.True(result);
            Assert.True(car.Speed < initialSpeed);
        }
        
        [Fact]
        public void ShiftToReverse_FailWhenMoving()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            car.ShiftGear(Gear.First);
            car.ReleaseClutch();
            car.Accelerate(0.3);
            
            
            car.PressClutch();
            var result = car.ShiftGear(Gear.Reverse);
            
            
            Assert.False(result);
            Assert.NotEqual(Gear.Reverse, car.CurrentGear);
        }
        
        [Fact]
        public void ShiftToReverse_SuccessWhenStopped()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            
            
            var result = car.ShiftGear(Gear.Reverse);
            
            
            Assert.True(result);
            Assert.Equal(Gear.Reverse, car.CurrentGear);
        }
        
        [Fact]
        public void ReleaseClutch_FailWhenSpeedTooLowForGear()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            car.ShiftGear(Gear.Third);
            
            
            var result = car.ReleaseClutch();
            
            
            Assert.False(result);
            Assert.Equal(ClutchState.Engaged, car.Clutch);
        }
        
        [Fact]
        public void GetStatus_ReturnsCorrectString()
        {
            
            var car = new ManualTransmissionCar();
            car.StartEngine();
            
            
            var status = car.GetStatus();
            
            
            Assert.Contains("Двигатель: Запущен", status);
            Assert.Contains("Передача: N", status);
            Assert.Contains("Сцепление: Выжато", status);
        }
    }
}