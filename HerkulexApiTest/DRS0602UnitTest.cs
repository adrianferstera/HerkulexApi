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
            myInterface = new HerkulexInterface("COM4", 57600);
            myServo = new HerkulexDrs0602(7, myInterface);
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
            myServo.SetColor(HerkulexColor.BLUE);
            var status = myServo.Status(); 
            Assert.IsTrue(status);
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
