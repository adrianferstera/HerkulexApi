using System;
using System.Collections.Generic;

namespace HerkulexApi
{
    /// <summary>
    /// Class to create a HerkuleX specific serial communication protocol. 
    /// </summary>
    public class HerkulexSerialCommand
    {
        public HerkulexCmd Cmd;
        public int MyServoId;


        /// <summary>
        /// Creates an instance of a serial command
        /// </summary>
        /// <param name="cmd">Command type</param>
        /// <param name="myServoId">Id which this command should be addressed to.</param>
        public HerkulexSerialCommand(HerkulexCmd cmd, int myServoId)
        {
            Cmd = cmd;
            MyServoId = myServoId;
        }

        private List<int> ConstructCommand()
        {
            var myCommand = new List<int>()
            {
                0xFF, 0xFF,0x00, MyServoId, Convert.ToByte(Cmd), 0x00, 0x00
            };
            return myCommand;
        }

        /// <summary>
        /// Construct the serial command
        /// </summary>
        /// <param name="optionalCommand">The optional data which should be send to the servo.</param>
        /// <returns></returns>
        public byte[] ConstructSerialProtocol(List<int> optionalCommand)
        {
            var myCommandHeader = ConstructCommand(); 
            myCommandHeader.AddRange(optionalCommand);
            var finalCommand = CalculateCheckSums(myCommandHeader);
            return finalCommand.ConvertAll(i => (byte) i).ToArray(); 
        }
        private int CheckSum1(List<int> sendData)
        {
            sendData[2] = sendData.Count;
            var checkSumVariables = sendData.GetRange(2, 3);
            //7 is the length of the header we do not need fo the checksum 
            checkSumVariables.AddRange(sendData.GetRange(7, sendData.Count - 7));
            var checkSum1 = 0;
            foreach (var el in checkSumVariables)
            {
                checkSum1 ^= el;
            }

            return checkSum1;
        }

        private List<int> CalculateCheckSums(List<int> sendData)
        {
            var checkSum1Variable = CheckSum1(sendData);
            var checkSum2 = (~(checkSum1Variable)) & 0xFE;
            var checkSum1 = checkSum1Variable & 0xFE;
            //add checksums to the List 
            sendData[5] = checkSum1;
            sendData[6] = checkSum2;
            return sendData;
        }

    }
}
