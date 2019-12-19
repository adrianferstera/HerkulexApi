using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace HerkulexApi
{
    public class HerkulexInterfaceConnector
    {
        public SerialPort MySerialPort;

        public HerkulexInterfaceConnector(string portName, int baudRate)
        {
            try
            {
                MySerialPort = new SerialPort(portName, baudRate);
                MySerialPort.ReadTimeout = 1000; 
                MySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Close()
        {
            try
            {
                MySerialPort.Close();
            }
            catch (Exception)
            {
                throw new Exception("Could not close the Serialport\n");
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                /*MySerialPort.DiscardInBuffer();
                MySerialPort.DiscardOutBuffer();*/
                MySerialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public void Send(IEnumerable<int> data)
        {
            var myConvertedArray = data.ToList().ConvertAll(i => (byte) i).ToArray(); 
            try
            {
                MySerialPort.Write(myConvertedArray, 0, myConvertedArray.Length);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public byte[] Read(int length)
        {
            
            byte[] myBuffer = new byte[length];
           //var values = MySerialPort.ReadLine();
            MySerialPort.Read(myBuffer, 0, length);
            return myBuffer; 
        }
        }
}
