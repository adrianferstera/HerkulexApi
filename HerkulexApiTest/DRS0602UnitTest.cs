
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HerkulexApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HerkulexApiTest
{
    [TestClass]
    public class DRS0602UnitTest
    {
        private HerkulexServo myServo1, myServo2, myServo3, myServo4, myServo5;
        private HerkulexInterfaceConnector myHerkulexInterface;
        private List<HerkulexServo> myServos; 

         [TestInitialize]
        public void InititalizeMotor()
        {
            myHerkulexInterface = new HerkulexInterfaceConnector("COM4", 115200);
            myServo1 = new HerkulexServo(1, myHerkulexInterface);
            myServo2 = new HerkulexServo(2, myHerkulexInterface);
            myServo3 = new HerkulexServo(3, myHerkulexInterface);
            myServo4 = new HerkulexServo(4, myHerkulexInterface);
            myServo5 = new HerkulexServo(5, myHerkulexInterface);
            myServos=new List<HerkulexServo>(){myServo1, myServo2, myServo3, myServo4, myServo5};
           // myServo1.NeutralPosition = 60;
            //myServo2.NeutralPosition = 60;
            
        }

        [TestCleanup]
        public void CleanUp()
        {
           // myServo1.Reboot();
           // myServo2.Reboot();
            myHerkulexInterface.Close();
        }

        [TestMethod]
        public void TurnMotor()
        {
            var numberCycles = 20; 
            myServo1.TorqueOn();
            myServo1.AccelerationRatio(0);
            myServo2.TorqueOn();
            myServo2.AccelerationRatio(0);
            myServo3.TorqueOn();
            myServo3.AccelerationRatio(0);
            myServo4.TorqueOn();
            myServo4.AccelerationRatio(0);
            myServo5.TorqueOn();
            myServo5.AccelerationRatio(0);
            Thread.Sleep(500);
            var watch = new Stopwatch(); 
            for (int i = 0; i < numberCycles; i++)
            {
                var task1 = new Task(() => myServo1.MoveServoPosition(-30, 500));
                var task2 = new Task(() => myServo2.MoveServoPosition(-30, 500));
                var task3 = new Task(() => myServo3.MoveServoPosition(-30, 500));
                var task4 = new Task(() => myServo4.MoveServoPosition(-30, 500));
                var task5 = new Task(() => myServo5.MoveServoPosition(-30, 500));
                var taskList1 = new List<Task>() { task4, task5 ,task1, task2, task3};
                watch.Start();
                foreach (var el in taskList1) el.Start();
                Task.WaitAll(taskList1.ToArray());
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
                watch.Reset();
                var task6 = new Task(() => myServo1.MoveServoPosition(30, 500));
                var task7 = new Task(() => myServo2.MoveServoPosition(30, 500));
                var task8 = new Task(() => myServo3.MoveServoPosition(30, 500));
                var task9 = new Task(() => myServo4.MoveServoPosition(30, 500));
                var task10 = new Task(() => myServo5.MoveServoPosition(30, 500));
                var taskList2 = new List<Task>() { task9, task10, task6, task7, task8 };
                foreach (var el in taskList2) el.Start();
                Task.WaitAll(taskList2.ToArray());
            }
            
        }

        [TestMethod]
        public void TestGuiMapper()
        {
           foreach (var servo in myServos)
            {
                servo.TorqueOn();
                servo.NeutralPosition = -60;
                servo.MoveToNeutralPosition();
            }
            var targetList = new List<Datapoint>();
            var replayer = new Replayer(-60, 60);
            replayer.StartSeries(HerkulexGuiMapper.WaveformType.SineTriangle, 1, 1, 1, 1, myServos);
        }

        [TestMethod]
        public void MoveToNeutralPosition()
        {
            myServo1.TorqueOn();
            myServo1.AccelerationRatio(10);
            myServo2.TorqueOn();
            myServo2.AccelerationRatio(10);
            myServo3.TorqueOn();
            myServo3.AccelerationRatio(10);
            myServo4.TorqueOn();
            myServo4.AccelerationRatio(10);
            myServo5.TorqueOn();
            myServo5.AccelerationRatio(10);
            myServo1.MoveServoPosition(60, 1000);
            myServo2.MoveServoPosition(60, 1000);
            myServo3.MoveServoPosition(60, 1000);
            myServo4.MoveServoPosition(60, 1000);
            myServo5.MoveServoPosition(60, 1000);
        }
       

        [TestMethod]
        public void Status()
        {
            var status1 = myServo1.Status();
            Assert.IsTrue(status1);
            var status2 = myServo2.Status();
            Assert.IsTrue(status2);
            var status3 = myServo3.Status();
            Assert.IsTrue(status3);
        }
        [TestMethod]
        public void ColorChange()
        {
            myServo1.SetColor(HerkulexColor.RED);
            myServo2.SetColor(HerkulexColor.GREEN);
            myServo3.SetColor(HerkulexColor.BLUE);
            myServo4.SetColor(HerkulexColor.RED);
            myServo5.SetColor(HerkulexColor.GREEN);
        }
        [TestMethod]
        public void ChangeId()
        {
            var success = myServo4.ChangeId(4); 
            Assert.IsTrue(success);
        }
        [TestMethod]
        public void ChangeBaudRate()
        {
            foreach (var servo in myServos)
            {
                servo.ChangeBaudRate(HerkulexBaudRate.DEFAULT115200);
            }
            myHerkulexInterface.Reopen(115200);
            var success = myServo1.Status(); 
            Assert.IsTrue(success);
        }

    }
}
