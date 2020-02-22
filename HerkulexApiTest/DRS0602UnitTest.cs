using HerkulexApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HerkulexApiTest
{
    [TestClass]
    public class Drs0602UnitTest
    {
        [TestMethod]
        public void MoveServo()
        {
            var myHerkulexInterface = new HerkulexInterface("COM4", 115200);
            var myServo = new HerkulexDrs0602(5, myHerkulexInterface);
            myServo.TorqueOn();
            myServo.MoveServoPosition(-40, 500);
            myServo.MoveServoPosition(40, 500);
            myServo.SetColor(HerkulexColor.BLUE);
            var status = myServo.Status(); 
            Assert.IsTrue(status);
            myHerkulexInterface.Close();
        }
    }
}
