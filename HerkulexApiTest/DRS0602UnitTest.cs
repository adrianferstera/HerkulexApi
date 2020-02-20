
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
        private HerkulexServo myServo1, myServo2, myServo3, myServo4, myServo5, myServo6, myServo7, myServo8;
        private HerkulexInterfaceConnector myHerkulexInterface12;
        private HerkulexInterfaceConnector myHerkulexInterface34;
        private HerkulexInterfaceConnector myHerkulexInterface56;
        private HerkulexInterfaceConnector myHerkulexInterface78;
        private List<HerkulexServo> myServos;
        private List<HerkulexInterfaceConnector> myInterfaces;


        [TestInitialize]
        public void InititalizeMotor()
        {
            var ports = HerkulexInterfaceConnector.AvailableSerialPorts();
            myHerkulexInterface12 = new HerkulexInterfaceConnector("COM11", 57600);
            myHerkulexInterface34 = new HerkulexInterfaceConnector("COM12", 57600);
            myHerkulexInterface56 = new HerkulexInterfaceConnector("COM13", 57600);
            myHerkulexInterface78 = new HerkulexInterfaceConnector("COM14", 57600);
            myServo1 = new HerkulexServo(1, myHerkulexInterface12);
            myServo2 = new HerkulexServo(2, myHerkulexInterface12);
            myServo3 = new HerkulexServo(3, myHerkulexInterface34);
            myServo4 = new HerkulexServo(4, myHerkulexInterface34);
            myServo5 = new HerkulexServo(5, myHerkulexInterface56);
            myServo6 = new HerkulexServo(6, myHerkulexInterface56);
            myServo7 = new HerkulexServo(7, myHerkulexInterface78);
            myServo8 = new HerkulexServo(8, myHerkulexInterface78);
            myServos = new List<HerkulexServo>() { myServo1, myServo2, myServo3, myServo4, myServo5, myServo6, myServo7, myServo8 };
           // myServos = new List<HerkulexServo>() { myServo5, myServo6, myServo7, myServo8 };
            myInterfaces = new List<HerkulexInterfaceConnector>() { myHerkulexInterface12, myHerkulexInterface34, myHerkulexInterface56, myHerkulexInterface78 };
            // myServo1.NeutralPosition = 60;
            //myServo2.NeutralPosition = 60;

        }

        [TestCleanup]
        public void CleanUp()
        {
            // myServo1.Reboot();
            // myServo2.Reboot();
            foreach (var myHerkulexInterface in myInterfaces)
            {
                myHerkulexInterface.Close();
            }
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
            myServo6.TorqueOn();
            myServo6.AccelerationRatio(0);
            myServo7.TorqueOn();
            myServo7.AccelerationRatio(0);
            myServo8.TorqueOn();
            myServo8.AccelerationRatio(0);
            Thread.Sleep(500);
            var watch = new Stopwatch();
            for (int i = 0; i < numberCycles; i++)
            {
                var task1 = new Task(() => myServo1.MoveServoPosition(-30, 500));
                var task2 = new Task(() => myServo2.MoveServoPosition(-30, 500));
                var task3 = new Task(() => myServo3.MoveServoPosition(-30, 500));
                var task4 = new Task(() => myServo4.MoveServoPosition(-30, 500));
                var task5 = new Task(() => myServo5.MoveServoPosition(-30, 500));
                var task6 = new Task(() => myServo6.MoveServoPosition(-30, 500));
                var task7 = new Task(() => myServo7.MoveServoPosition(-30, 500));
                var task8 = new Task(() => myServo8.MoveServoPosition(-30, 500));
                var taskList1 = new List<Task>() { task1, task2, task3, task4, task5, task6, task7, task8 };
                //var taskList1 = new List<Task>() {task1, task2, task3, task4};

                foreach (var el in taskList1)
                {
                    el.Start();
                    if (el != taskList1.Last())
                    {
                        Thread.Sleep(125);
                    }
                }
                //Parallel.ForEach(taskList1, task => task.Start());
                Task.WaitAll(taskList1.ToArray());
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
                watch.Reset();
                var task9 = new Task(() => myServo1.MoveServoPosition(30, 500));
                var task10 = new Task(() => myServo2.MoveServoPosition(30, 500));
                var task11 = new Task(() => myServo3.MoveServoPosition(30, 500));
                var task12 = new Task(() => myServo4.MoveServoPosition(30, 500));
                var task13 = new Task(() => myServo5.MoveServoPosition(30, 500));
                var task14 = new Task(() => myServo6.MoveServoPosition(30, 500));
                var task15 = new Task(() => myServo7.MoveServoPosition(30, 500));
                var task16 = new Task(() => myServo8.MoveServoPosition(30, 500));
                var taskList2 = new List<Task>() { task9, task10, task11, task12, task13, task14, task15, task16 };
                foreach (var el in taskList2)
                {
                    el.Start();
                    if (el != taskList2.Last())
                    {
                        Thread.Sleep(125);
                    }
                    
                }
                // var taskList2 = new List<Task>() { task9, task10, task11, task12 };
                //Parallel.ForEach(taskList2, task => task.Start());
                Task.WaitAll(taskList2.ToArray());
            }

        }

        [TestMethod]
        public void TestGuiMapper()
        {
            foreach (var servo in myServos)
            {
                servo.TorqueOn();
                servo.NeutralPosition = -30;
                servo.MoveToNeutralPosition();
            }
            var targetList = new List<Datapoint>();
            var replayer = new Replayer(-30, 30);
            replayer.StartSeries(HerkulexGuiMapper.WaveformType.Sine, 1, 1, 1, 10, myServos);
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
            myServo6.TorqueOn();
            myServo6.AccelerationRatio(10);
            myServo7.TorqueOn();
            myServo7.AccelerationRatio(10);
            myServo8.TorqueOn();
            myServo8.AccelerationRatio(10);
            myServo1.MoveServoPosition(0, 1000);
            myServo2.MoveServoPosition(0, 1000);
            myServo3.MoveServoPosition(0, 1000);
            myServo4.MoveServoPosition(0, 1000);
            myServo5.MoveServoPosition(0, 1000);
            myServo6.MoveServoPosition(0, 1000);
            myServo7.MoveServoPosition(0, 1000);
            myServo8.MoveServoPosition(0, 1000);
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
            myServo3.SetColor(HerkulexColor.GREEN);
            myServo4.SetColor(HerkulexColor.RED);
            myServo5.SetColor(HerkulexColor.GREEN);
            myServo6.SetColor(HerkulexColor.BLUE);
            myServo7.SetColor(HerkulexColor.RED);
            myServo8.SetColor(HerkulexColor.BLUE);

        }
        [TestMethod]
        public void ChangeId()
        {
            var success = myServo8.ChangeId(8);
            Assert.IsTrue(success);
        }
        [TestMethod]
        public void ChangeBaudRate()
        {
            foreach (var servo in myServos)
            {
                servo.ChangeBaudRate(HerkulexBaudRate.RATE57600);
            }

            foreach (var myInterface in myInterfaces) myInterface.Reopen(115200);
            var success = myServo1.Status();
            Assert.IsTrue(success);
        }

    }
}
