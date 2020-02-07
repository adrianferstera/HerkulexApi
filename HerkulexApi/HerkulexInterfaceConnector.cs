using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace HerkulexApi
{
    public class HerkulexInterfaceConnector
    {
        public SerialPort MySerialPort;
        public bool IsOpen => MySerialPort.IsOpen;
        private string portName;
        private int baudRate; 

        public HerkulexInterfaceConnector(string portName, int baudRate)
        {
            try
            {
                this.baudRate = baudRate;
                this.portName = portName; 
                MySerialPort = new SerialPort(portName, baudRate);
                MySerialPort.ReadTimeout = 5000; 
                MySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Reopen(int baudRate)
        {
            this.baudRate = baudRate; 

            try
            {
                MySerialPort.Close();
                Thread.Sleep(100);
                MySerialPort = new SerialPort(portName, baudRate);
                MySerialPort.ReadTimeout = 5000;
                MySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Reopen()
        {
            try
            {
                MySerialPort.Close();
                Thread.Sleep(100);
                MySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public static string[] AvailableSerialPorts()
        {
            var availableSerialPorts = SerialPort.GetPortNames(); 
            return availableSerialPorts;
        }

        public void Close()
        {
            if (MySerialPort.IsOpen)
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
        }

        public void Send(byte[] data)
        {
            try
            {
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
            MySerialPort.DiscardInBuffer();
            MySerialPort.DiscardOutBuffer();
            byte[] myBuffer = new byte[length];
           //var values = MySerialPort.ReadLine();
            MySerialPort.Read(myBuffer, 0, length);
            return myBuffer; 
        }
        public byte[] SendAndRead(int length, byte[] data)
        {
            MySerialPort.DiscardInBuffer();
            MySerialPort.DiscardOutBuffer();
            Reopen();
            byte[] myBuffer = new byte[data.Length+length];
            Thread.Sleep(500);
            try
            {
                MySerialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
            Thread.Sleep(100);
            try
            {
                MySerialPort.Read(myBuffer, 0, myBuffer.Length);
            }
            catch (TimeoutException e)
            {
                throw new TimeoutException("Could not communicate with the servo(s). Please check: " +
                                           "if you have selected the correct COM Port, " +
                                           "if the servo(s) are connected or " +
                                           "if the servo(s) have power, " +
                                           "if that has not solved your problem, you should contact the software engineer.");
            }
            catch (Exception e)
            {
                throw e; 
            }
            return myBuffer;
        }
    }
}
