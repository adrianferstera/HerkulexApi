using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HerkulexApi
{
    public class HerkulexDrs0602 : IHerkulexServo
    {
        public int Id { get; private set;  }
        public double MaxSpeed => 0.00274;
        public int MaxAccRatio => 80;
        public int MinAccRatio => 0;

        public int NeutralPosition
        {
            get => neutralPos;
            set
            {
                if (value < MaxDegrees && value > MinDegrees)
                {
                    neutralPos = value;
                }
            }
        }
        private const int MaxServoPosition = 22129;
        private const int MinServoPosition = 10627;
        private const double MaxDegrees = 159.6;
        private const double MinDegrees = -159.9;

        private int neutralPos = 0;
        private const double DegreesRange = MaxDegrees - MinDegrees;

        private const double MsPerCount = 11.2;
        private const double MaximumAccTime = MsPerCount * 255;
        private readonly HerkulexInterface myHerkulexInterface;
        private double lastPosition = 0;
        private int accRatio = 60;
        private Dictionary<HerkulexCmd, ACKPackage> cmdAckDictionary => CmdAckDictionary();



        public HerkulexDrs0602(int id, HerkulexInterface herkulexInterface)
        {
            Id = id;
            myHerkulexInterface = herkulexInterface;
            TorqueOn();
            AccelerationRatio(accRatio);
            Thread.Sleep(500);
            TorqueOff();
        }

        public void MoveToNeutralPosition()
        {
            MoveServoPosition(neutralPos, 1000);
        }

        public void TorqueOn()
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() {(int) Torque.RAM, 0x01, (int) Torque.ON};
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));

        }

        public void TorqueOff()
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() {(int) Torque.RAM, 0x01, (int) Torque.OFF};
           Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader)); 
        }


        private int convert2PosForServo(double degrees)
        {
            var positionForServo = (Convert.ToDouble(MaxServoPosition - MinServoPosition)
                                    / DegreesRange) * (degrees - MinDegrees) + MinServoPosition;
            return Convert.ToInt32(positionForServo);
        }

        public void SetColor(HerkulexColor color)
        {
            var command = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var finalCommand = command.ConstructSerialProtocol(new List<int>() {53, 0x01, (byte) color});//Address of color is 53
            Send2Servo(finalCommand);
        }

        public void MoveServoPosition(double position, int playTime)
        {
            var myPlayTime = playTime;
            if (Math.Abs(position) > MaxDegrees)
                throw new InvalidOperationException("Your desired position is out of range");
            if (myPlayTime > MaximumAccTime) myPlayTime = Convert.ToInt32(MaximumAccTime);
            var travelingTime = Convert.ToInt32(Math.Abs(position - lastPosition) * MaxSpeed * 1000); //milliseconds
            if (travelingTime > myPlayTime)
            {
                myPlayTime = travelingTime;
            }

            var playTimeForServo = Convert.ToInt32(Convert.ToDouble(myPlayTime) / MsPerCount);

            var servoPos = convert2PosForServo(position);
            var lsb = servoPos;
            var msb = servoPos >> 8;
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.S_JOG_REQ, Id);

            var myCommandHeader = new List<int>() {playTimeForServo, lsb, msb, 0x04, Id}; //0x04 stands for color green
           var sleepingTime = Convert.ToInt32(myPlayTime + 10);
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
            lastPosition = position;
            Thread.Sleep(sleepingTime);
        }

        public bool Status()
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.STAT_REQ, Id);
            var myCommandHeader = new List<int>();
            var request = myCommand.ConstructSerialProtocol(myCommandHeader);
            byte[] answer;
            try
            {
                answer = myHerkulexInterface.SendAndRead(20, request);
            }
            catch (Exception e)
            {
                throw e;
            }

            var success = ProcessHerkulexPackage(answer, HerkulexCmd.STAT_REQ, out var processedPackage);
            //things to do error checking for all errors according to page 41
            if (success)
            {
                if (processedPackage[processedPackage.Count - 2] == 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void PlaySeries(IEnumerable<HerkulexDatapoint> targets)
        {
            foreach (var target in targets)
            {
                //target = minimum/maximum points of the waveform
                if (accRatio != target.AccelerationRatio)
                {
                    AccelerationRatio(target.AccelerationRatio);
                }
                var t = Convert.ToInt32(target.XValue); 
                MoveServoPosition(target.YValue, t);
            }
        }

        public void Reboot()
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.REBOOT_REQ, Id);
            var myCommandHeader = new List<int>();
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
        }

        public void ChangeBaudRate(HerkulexBaudRate baudRate)
        {
            //After u used this method, close the serial port and reopen it with the new baud rate 
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.EEP_WRITE_REQ, Id);
            var myCommandHeader = new List<int>(){(int)Ram.BAUD_RATE_EEP,0x01, (int)baudRate };
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
            Reboot();
        }

        public bool ChangeId(int newId)
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.EEP_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() { (int)Ram.SERVO_ID_EEP, 0x01, (int)newId };
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
            Thread.Sleep(1000);
            Reboot();
            Id = newId;
            Thread.Sleep(2000);
            var status = Status();
            if (status) return true;
            return false; 
        }


        public void AccelerationRatio(int ratio)
        {
            //higher the ratio, smoother the acceleration 
            if (ratio > MaxAccRatio) this.accRatio = MaxAccRatio;
            else if (ratio < MinAccRatio) this.accRatio = MinAccRatio;
            else this.accRatio = ratio;
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() { (int)Ram.ACCELERATION_RATIO_EEP, 1, ratio };
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
        }

        private void Send2Servo(byte[] data)
        {
            try
            {
                myHerkulexInterface.Send(data);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Could not find or lost connection servo with id {Id}.");
            }
            catch (TimeoutException)
            {
                throw new TimeoutException($"The servo with id {Id} did not answer.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private bool ProcessHerkulexPackage(byte[] packages, HerkulexCmd request, out List<byte> package)
        {
            package = new List<byte>();
            var answer = cmdAckDictionary[request];
            var listPackage = packages.ToList();
            if (listPackage.Contains((byte)answer))
            {
                //look into manual Chapter 6,P.35 for command examples
                var index = listPackage.IndexOf((byte)answer);
                var packageSize = listPackage[index - 2];
                package = listPackage.GetRange(index - 4, packageSize);
                return true; 
            }
            return false; 
        }

        private Dictionary<HerkulexCmd, ACKPackage> CmdAckDictionary()
        {
            var herkulexCmd = Enum.GetValues(typeof(HerkulexCmd)).Cast<HerkulexCmd>();
            var ackRespones = Enum.GetValues(typeof(ACKPackage)).Cast<ACKPackage>();
            var Dict = herkulexCmd.Zip(ackRespones, (k, v) => new { k, v }).ToDictionary(el => el.k, el => el.v);
            return Dict;
        }
    }
}
