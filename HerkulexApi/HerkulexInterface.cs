using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace HerkulexApi
{
    /// <summary>
    /// Communication interface to connect the HerkuleX DRS 0602 with HerkulexDrs0602 class
    /// </summary>
    public class HerkulexInterface
    {
        private SerialPort mySerialPort;
        public bool IsOpen => mySerialPort.IsOpen;
        /// <summary>
        /// The port name which the interface is connected to. 
        /// </summary>
        public readonly string PortName;
        /// <summary>
        /// Baud rate which the interface and this class communicate. 
        /// </summary>
        public int BaudRate; 

        /// <summary>
        /// Initializes an instance of HerkuleX Communication interface
        /// </summary>
        /// <param name="portName">Port name which the interface is connected to. </param>
        /// <param name="baudRate">Baud rate which should be used to communicate with the interface.</param>
        /// <exception cref="Exception"></exception>
        public HerkulexInterface(string portName, int baudRate)
        {
            try
            {
                this.BaudRate = baudRate;
                this.PortName = portName; 
                mySerialPort = new SerialPort(portName, baudRate);
                mySerialPort.ReadTimeout = 5000; 
                mySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Reopen the port. 
        /// </summary>
        /// <param name="baudRate">The new baud rate which should be used for communication. </param>
        /// <exception cref="Exception">If there occurs an error.</exception>
        public void Reopen(int baudRate)
        {
            this.BaudRate = baudRate; 

            try
            {
                mySerialPort.Close();
                Thread.Sleep(100);
                mySerialPort = new SerialPort(PortName, baudRate);
                mySerialPort.ReadTimeout = 5000;
                mySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Reopen the port. 
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Reopen()
        {
            try
            {
                mySerialPort.Close();
                Thread.Sleep(100);
                mySerialPort.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        /// <summary>
        /// Scans all serial ports which are connected to this computer. 
        /// </summary>
        /// <returns>An array with all possible serial ports.</returns>
        public static string[] AvailableSerialPorts()
        {
            var availableSerialPorts = SerialPort.GetPortNames().ToList();
            availableSerialPorts.Sort();
            return availableSerialPorts.ToArray();
        }

        /// <summary>
        /// Closes the port. 
        /// </summary>
        /// <exception cref="Exception">Throws an exception if there occurs an error. </exception>
        public void Close()
        {
            if (mySerialPort.IsOpen)
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
        }

        /// <summary>
        /// Send an array of data to the port.
        /// </summary>
        /// <param name="data">The array of data.</param>
        /// <exception cref="Exception"></exception>
        public void Send(byte[] data)
        {
            try
            {
                mySerialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        /// <summary>
        /// Reads from the port.
        /// </summary>
        /// <param name="length">The maximum length until when it shoyuld read from the port.</param>
        /// <returns>An array of the data from the port.</returns>
        public byte[] Read(int length)
        {
            mySerialPort.DiscardInBuffer();
            mySerialPort.DiscardOutBuffer();
            byte[] myBuffer = new byte[length];
            mySerialPort.Read(myBuffer, 0, length);
            return myBuffer; 
        }
        /// <summary>
        /// Sends and directly reads from the port.
        /// </summary>
        /// <param name="length">The maximum length until when it shoyuld read from the port</param>
        /// <param name="data">The array of data to be send</param>
        /// <returns></returns>
        /// <exception cref="Exception">If during the communication occured an error.</exception>
        /// <exception cref="TimeoutException">If the device did not answer.</exception>
        public byte[] SendAndRead(int length, byte[] data)
        {
            mySerialPort.DiscardInBuffer();
            mySerialPort.DiscardOutBuffer();
            Reopen();
            byte[] myBuffer = new byte[data.Length+length];
            Thread.Sleep(500);
            try
            {
                mySerialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
            Thread.Sleep(100);
            try
            {
                mySerialPort.Read(myBuffer, 0, myBuffer.Length);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException("Could not communicate with the Ports. Please check: " +
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
