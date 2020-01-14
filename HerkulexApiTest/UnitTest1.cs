
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HerkulexApi;
using HerkulexGuiMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HerkulexApiTest
{
    [TestClass]
    public class UnitTest1
    {
        private HerkulexServo myServo;
        private HerkulexServo myServo1;
        private HerkulexInterfaceConnector myHerkulexInterface; 

         [TestInitialize]
        public void InititalizeMotor()
        {
            myHerkulexInterface = new HerkulexInterfaceConnector("COM4", 57600);
            myServo = new HerkulexServo(219, myHerkulexInterface);
            myServo1 = new HerkulexServo(218, myHerkulexInterface);
            myServo.NeutralPosition = 60;
            myServo1.NeutralPosition = 60; 
        }

        [TestCleanup]
        public void CleanUp()
        {
            myServo.Reboot();
            myServo1.Reboot();
            myHerkulexInterface.Close();
        }

        [TestMethod]
        public void TurnMotor()
        {
            myServo.TorqueOn();
            myServo.AccelerationRatio(0);
            myServo1.TorqueOn();
            myServo1.AccelerationRatio(0);
            var watch = new Stopwatch(); 
            for (int i = 0; i < 20; i++)
            {
                var task1 = new Task(() => myServo.MoveServoPosition(-30, 50));
                var task2 = new Task(() => myServo1.MoveServoPosition(-30, 50));
                var taskList1 = new List<Task>() { task1, task2 };
                watch.Start();
                task1.Start();
                task2.Start();
                Task.WaitAll(taskList1.ToArray());
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
                watch.Reset();
                var task3 = new Task(() => myServo.MoveServoPosition(30, 200));
                var task4 = new Task(() => myServo1.MoveServoPosition(30, 200));
                var taskList2 = new List<Task>() { task3, task4 };
                task3.Start();
                task4.Start();
                Task.WaitAll(taskList2.ToArray());
            }
            
        }

        [TestMethod]
        public void PlayWaveForm()
        {
            myServo.TorqueOn();
            myServo.AccelerationRatio(40);
            myServo1.TorqueOn();
            myServo1.AccelerationRatio(40);
            var servoList = new List<HerkulexServo>(){myServo, myServo1};
            var targetList = new List<Datapoint>();
            targetList.Add(new Datapoint(0, 0.2));
            targetList.Add(new Datapoint(0.2, 0.8));
            targetList.Add(new Datapoint(0.4, 0.2));
            targetList.Add(new Datapoint(0.6, 0.8));
            targetList.Add(new Datapoint(0.8, 0.2));
            targetList.Add(new Datapoint(1, 0.8));
            var replayer = new Replayer(-60, 60);
            replayer.StartSeries(targetList, 1, 1, 1, 1, servoList);
        }

        [TestMethod]
        public void MoveToNeutralPosition()
        {
            myServo.TorqueOn();
            myServo.AccelerationRatio(10);
            myServo1.TorqueOn();
            myServo1.AccelerationRatio(10);
            myServo.MoveServoPosition(-60, 1000);
            myServo1.MoveServoPosition(60, 1000);
        }
        [TestMethod]
        public void PlayWaveFormOneServo()
        {
            myServo.TorqueOn();
            myServo.MoveToNeutralPosition();
            myServo.AccelerationRatio(10);
           
            var targetList = new List<double>();

            targetList.Add(60);
            targetList.Add(-60);
            targetList.Add(60);
            targetList.Add(-60);
            targetList.Add(60);
            targetList.Add(-60);

            myServo.PlaySeries(targetList, 400);
        }

        [TestMethod]
        public void Status()
        {
            var status1 = myServo.Status();
            var status2 = myServo1.Status();
            Assert.IsTrue(status1);
            Assert.IsTrue(status2);
        }

    }
}
