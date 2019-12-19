using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Herkulex0602Api
{
    public class HerkulexServo
    {
        public readonly int Id; 

        public HerkulexServo(int id)
        {
            Id = id; 
        }
            private int CheckSum1(byte[] sendData, int dataLength)
            {
            var packetSize = 7 + dataLength;
            var checkSum1 = packetSize ^ (int)HerkulexCmd.RAM_WRITE_REQ ^ 219; 
            foreach (var el in sendData)
            {
                checkSum1 ^= el; 
            }

            return checkSum1 & 0xFE; 
        }

        private int CheckSum2(int checksum1)
        {
            return (~checksum1) & 0xFE; 
        }

        public void SendData(SerialPort mySerialPort, byte[] sendData)
        {

            try
            {
                mySerialPort.Write(sendData, 0, sendData.Length);
            }
            catch (Exception)
            {
                throw new Exception("Could not write to the Servo");
            }
           
        }

    }
}
