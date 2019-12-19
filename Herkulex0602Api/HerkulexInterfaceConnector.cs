using System;
using System.IO.Ports;



namespace Herkulex0602Api
{
    public class HerkulexInterfaceConnector
    {
        public SerialPort mySerialPort;

        public HerkulexInterfaceConnector(string portName, int baudRate)
        {
            try
            {
                mySerialPort = new SerialPort(portName, baudRate);
                mySerialPort.Open();
            }
            catch (Exception)
            {
                throw new Exception("Could not open the Serialport\n");
            }
        }

        public void Close()
        {
            try
            {
                mySerialPort.Close();
            }
            catch (Exception)
            {
                throw new Exception("Could not close the Serialport\n");
            }
        }

        public void SendString(string data)
        {
            try
            {
                mySerialPort.Write(data);
            }
            catch (Exception)
            {
                throw new Exception("Could not write to the Serialport");
            }
            
        }


    }
}
