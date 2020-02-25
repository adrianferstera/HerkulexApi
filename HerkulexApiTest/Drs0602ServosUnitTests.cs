using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HerkulexApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HerkulexApiTest
{
    [TestClass]
    public class Drs0602ServosUnitTests
    {
        
        private HerkulexInterface myHerkulexInterface12;
        private HerkulexInterface myHerkulexInterface34;
        private HerkulexInterface myHerkulexInterface56;
        private HerkulexInterface myHerkulexInterface78;
        private List<HerkulexInterface> myInterfaces;

        private List<IHerkulexServo> myServos;
        private HerkulexDrs0602 myServo1, myServo2, myServo3, myServo4, myServo5, myServo6, myServo7, myServo8;

        [TestInitialize]
        public void InititalizeMotor()
        {
            myHerkulexInterface12 = new HerkulexInterface("COM11", 57600);
            myHerkulexInterface34 = new HerkulexInterface("COM12", 57600);
            myHerkulexInterface56 = new HerkulexInterface("COM13", 57600);
            myHerkulexInterface78 = new HerkulexInterface("COM14", 57600);
            myServo1 = new HerkulexDrs0602(1, myHerkulexInterface12);
            myServo2 = new HerkulexDrs0602(2, myHerkulexInterface12);
            myServo3 = new HerkulexDrs0602(3, myHerkulexInterface34);
            myServo4 = new HerkulexDrs0602(4, myHerkulexInterface34);
            myServo5 = new HerkulexDrs0602(5, myHerkulexInterface56);
            myServo6 = new HerkulexDrs0602(6, myHerkulexInterface56);
            myServo7 = new HerkulexDrs0602(7, myHerkulexInterface78);
            myServo8 = new HerkulexDrs0602(8, myHerkulexInterface78);
            myServos = new List<IHerkulexServo>() { myServo1, myServo2, myServo3, myServo4, myServo5, myServo6, myServo7, myServo8 };
            myInterfaces = new List<HerkulexInterface>() { myHerkulexInterface12, myHerkulexInterface34, myHerkulexInterface56, myHerkulexInterface78 };
        }

        [TestCleanup]
        public void CleanUp()
        {
            foreach (var myHerkulexInterface in myInterfaces)
            {
                myHerkulexInterface.Close();
            }
        }

        [TestMethod]
        public void TurnMotorAsync()
        {
            foreach (var servo in myServos)
            {
                servo.TorqueOn();
                servo.AccelerationRatio(0);
            }
            var numberCycles = 20;
            Thread.Sleep(500);
            for (int i = 0; i < numberCycles; i++)
            {
                var threadList = new List<Thread>();
                foreach (var servo in myServos)
                {
                    var thread = new Thread(() => servo.MoveServoPosition(-30, 500));
                    threadList.Add(thread);
                }

                //start all threads together without an delay
                Parallel.ForEach(threadList, thread => thread.Start());

                //wait for all threads
                foreach (var thread in threadList) thread.Join();

                threadList.Clear();
                foreach (var servo in myServos)
                {
                    var thread = new Thread(() => servo.MoveServoPosition(30, 500));
                    threadList.Add(thread);
                }
                //start all threads together without an delay
                Parallel.ForEach(threadList, thread => thread.Start());

                //wait for all threads
                foreach (var thread in threadList) thread.Join();
            }

            foreach (var servo in myServos)
            {
                var status = servo.Status(); 
                Assert.IsTrue(status);
            }

        }

        [TestMethod]
        public void MoveToPosition()
        {
            foreach (var servo in myServos)
            {
                servo.TorqueOn();
                servo.AccelerationRatio(10);
                servo.MoveServoPosition(0, 1000);
                var status = servo.Status(); 
                Assert.IsTrue(status);
            }

        }


        [TestMethod]
        public void Status()
        {
            foreach (var servo in myServos)
            {
                var status = servo.Status(); 
                Assert.IsTrue(status);
            }
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
        

    }
}
