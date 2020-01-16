using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HerkulexApi
{
    public class HerkulexServo : IServo
    {
        public int Id { get; }
        public readonly double MaxSpeed = 0.00274; // [s/degree]
        public readonly int MaxAccRatio = 80;
        public readonly int MinAccRatio = 0;
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
        private readonly HerkulexInterfaceConnector MyConnector;
        private double LastPos = 0;
        private int AccRatio = 60;
        private Dictionary<HerkulexCmd, ACKPackage> cmdAckDictionary => CmdAckDictionary();



        public HerkulexServo(int id, HerkulexInterfaceConnector myConnector)
        {
            Id = id;
            this.MyConnector = myConnector;
            TorqueOn();
            AccelerationRatio(AccRatio);
            //MoveToNeutralPosition();
            Thread.Sleep(500);
            TorqueOff();
        }

        public void MoveToNeutralPosition()
        {
            MoveServoPosition(neutralPos, 1000);
        }

        public void TorqueOn()
        {
            var myCommand = new HerkulexCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() { (int)Torque.RAM, 0x01, (int)Torque.ON };
            MyConnector.Send(myCommand.ConstructMyCommand(myCommandHeader));
        }

        public void TorqueOff()
        {
            var myCommand = new HerkulexCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() { (int)Torque.RAM, 0x01, (int)Torque.OFF };
            MyConnector.Send(myCommand.ConstructMyCommand(myCommandHeader));
        }

       
        private int convert2PosForServo(double degrees)
        {
            var positionForServo = (Convert.ToDouble(MaxServoPosition - MinServoPosition)
                                    / DegreesRange) * (degrees - MinDegrees) + MinServoPosition;
            return Convert.ToInt32(positionForServo);
        }

        public void SetColor(HerkulexColor color)
        {
            var command = new HerkulexCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var finalCommand = command.ConstructMyCommand(new List<int>() {53, 0x01,(byte) color});
            try
            {
                MyConnector.Send(finalCommand);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void MoveServoPosition(double position, int playTime)
        {
            var myPlayTime = playTime;
            if (Math.Abs(position) > MaxDegrees) throw new InvalidOperationException("Your desired position is out of range");
            if (myPlayTime > MaximumAccTime) myPlayTime = Convert.ToInt32(MaximumAccTime);
            var travelingTime = Convert.ToInt32(Math.Abs(position - LastPos) * MaxSpeed * 1000); //milliseconds
            if (travelingTime > myPlayTime)
            {
                myPlayTime = travelingTime;
            }
            var playTimeForServo = Convert.ToInt32(Convert.ToDouble(myPlayTime) / MsPerCount);

            var servoPos = convert2PosForServo(position);
            var lsb = servoPos;
            var msb = servoPos >> 8;
            var myCommand = new HerkulexCommand(HerkulexCmd.S_JOG_REQ, Id);
           
            var myCommandHeader = new List<int>() { playTimeForServo, lsb, msb,0x04, Id }; //0x04 stands fo color
            var accelerationTime = Convert.ToDouble(myPlayTime) * Convert.ToDouble(AccRatio) * 0.01;
            //var sleepingTime = Convert.ToInt32(myPlayTime + 2 * accelerationTime);
            var sleepingTime = Convert.ToInt32(myPlayTime + 10);
            try
            {
                MyConnector.Send(myCommand.ConstructMyCommand(myCommandHeader));
            }
            catch (Exception e)
            {
                throw e;
            }
           
            LastPos = position;
            Thread.Sleep(sleepingTime);
        }

        public bool Status()
        {
            var myCommand = new HerkulexCommand(HerkulexCmd.STAT_REQ, Id);
            var myCommandHeader = new List<int>();
            var request = myCommand.ConstructMyCommand(myCommandHeader);
            byte[] answer; 
            try
            {
                answer = MyConnector.SendAndRead(20, request);
            }
            catch (Exception e)
            {
                throw e; 
            }
            var success= ProcessHerkulexPackage(answer, HerkulexCmd.STAT_REQ, out var processedPackage);
            //things to do 
            if (success)
            {
                if (processedPackage[processedPackage.Count - 1] == 0 && processedPackage.Last() == 0)
                {
                    return true;
                }
                return false; 
            }
            return false;
        }

        public void PlaySeries(IEnumerable<double> targets, int playTime)
        {
            foreach (var target in targets)
            {
                MoveServoPosition(target, playTime);
            }
        }

        public void Reboot()
        {
            var myCommand = new HerkulexCommand(HerkulexCmd.REBOOT_REQ, Id);
            var myCommandHeader = new List<int>();
            MyConnector.Send(myCommand.ConstructMyCommand(myCommandHeader));
        }

        public void AccelerationRatio(int ratio)
        {
            if (ratio > MaxAccRatio) this.AccRatio = MaxAccRatio;
            else if (ratio < MinAccRatio) this.AccRatio = MinAccRatio;
            else this.AccRatio = ratio;
            var myCommand = new HerkulexCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() { (int)Ram.ACCELERATION_RATIO_EEP, 1, ratio };
            try
            {
                MyConnector.Send(myCommand.ConstructMyCommand(myCommandHeader));
            }
            catch (Exception e)
            {
                throw e;
            }
            Thread.Sleep(100);
        }

        private bool ProcessHerkulexPackage(byte[] packages, HerkulexCmd request, out List<byte> package)
        {
            package = new List<byte>();
            var answer = cmdAckDictionary[request];
            var listPackage = packages.ToList();
            var i = 0;
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

        public Dictionary<HerkulexCmd, ACKPackage> CmdAckDictionary()
        {
            var herkulexCmd = HerkulexCmd.GetValues(typeof(HerkulexCmd)).Cast<HerkulexCmd>();
            var ackRespones = ACKPackage.GetValues(typeof(ACKPackage)).Cast<ACKPackage>();
            var Dict = herkulexCmd.Zip(ackRespones, (k, v) => new { k, v }).ToDictionary(el => el.k, el => el.v);
            return Dict;
        }
    }
}
