using System.Threading;
using HerkulexApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HerkulexApiTest
{
    [TestClass]
    public class Drs0602UnitTest

    {
        private HerkulexDrs0602 myServo;
        private HerkulexInterface myInterface; 
        [TestInitialize]
        public void StartUp()
        {
            var ports = HerkulexInterface.AvailableSerialPorts(); 
            //default baudRate is 112500
            myInterface = new HerkulexInterface("COM12", 57600);
            //default id: 219
            myServo = new HerkulexDrs0602(3, myInterface);

        }
        [TestCleanup]
        public void CleanUp()
        {
            myInterface.Close();
        }

        [TestMethod]
        public void MoveServo()
        {
            myServo.TorqueOn();
            myServo.MoveServoPosition(-40, 500);
            myServo.MoveServoPosition(0, 500);
        }
        [TestMethod]
        public void Status()
        {
           var status =  myServo.Status();
           Assert.IsTrue(status);
        }
        [TestMethod]
        public void ColorChange()
        {
            myServo.SetColor(HerkulexColor.RED);
            Thread.Sleep(1500);
            myServo.SetColor(HerkulexColor.BLUE);
            Thread.Sleep(1500);
            myServo.SetColor(HerkulexColor.GREEN);
            Thread.Sleep(1500);
        }

        [TestMethod]
        public void ChangeId()
        {
            var success = myServo.ChangeId(7);
            Assert.IsTrue(success);
        }
        [TestMethod]
        public void ChangeBaudRate()
        {
            myServo.ChangeBaudRate(HerkulexBaudRate.RATE57600);
        }
    }
}
