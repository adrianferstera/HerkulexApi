using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HerkulexApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HerkulexApiTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var myHerkulexInterface = new HerkulexInterfaceConnector("COM4", 115200);
            var myServo1 = new HerkulexServo(5, myHerkulexInterface);
            myServo1.TorqueOn();
            myServo1.MoveServoPosition(-40, 500);
            myServo1.MoveServoPosition(40, 500);
            myServo1.SetColor(HerkulexColor.BLUE);
            // myServo1.ChangeId(5);
            //var status1 = myServo1.Status();
            //Assert.IsTrue(status1);
            myHerkulexInterface.Close();
        }

        [TestMethod]
        public void SeriesPlayer()
        {
            var myHerkulexInterface12 = new HerkulexInterfaceConnector("COM11", 57600);
            var myHerkulexInterface34 = new HerkulexInterfaceConnector("COM12", 57600);
            var myHerkulexInterface56 = new HerkulexInterfaceConnector("COM13", 57600);
            var myHerkulexInterface78 = new HerkulexInterfaceConnector("COM14", 57600);
            var myServo1 = new HerkulexServo(1, myHerkulexInterface12);
            var myServo2 = new HerkulexServo(2, myHerkulexInterface12);
            var myServo3 = new HerkulexServo(3, myHerkulexInterface34);
            var myServo4 = new HerkulexServo(4, myHerkulexInterface34);
            var myServo5 = new HerkulexServo(5, myHerkulexInterface56);
            var myServo6 = new HerkulexServo(6, myHerkulexInterface56);
            var myServo7 = new HerkulexServo(7, myHerkulexInterface78);
            var myServo8 = new HerkulexServo(8, myHerkulexInterface78);
            var myServos = new List<HerkulexServo>()
            {
                myServo1, myServo2, myServo3, myServo4, myServo5, myServo6, myServo8
            };
            foreach (var servo in myServos)
            {
                servo.TorqueOn();
            }

            var datapoint1 = new Datapoint(500, 30);
            var datapoint2 = new Datapoint(500, -30);
            var datapoint3 = new Datapoint(500, 30);
            var datapoint4 = new Datapoint(500, -30);
            var targetList = new List<Datapoint>(){datapoint1, datapoint2, datapoint3, datapoint4};
            var task1 = new Task(() =>myServo1.PlaySeries(targetList));
            var task2 = new Task(() => myServo2.PlaySeries(targetList));
            var task3 = new Task(() => myServo3.PlaySeries(targetList));
            var task4 = new Task(() => myServo4.PlaySeries(targetList));
            var taskList = new List<Task>(){task1,task2, task3, task4};
            foreach (var task in taskList)
            {
                task.Start();
            }
           // Parallel.ForEach(taskList, task => task.Start());
            Task.WaitAll(taskList.ToArray()); 

        }
    }
}
