using System;
using System.IO.Ports;
using Herkulex0602Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herkulex0602ApiTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var myConnector = new HerkulexInterfaceConnector("COM4", 57600);
            myConnector.Close();
        }
    }
}
