using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HerkulexApi
{
    /// <summary>
    /// A class to programmatically control a HerkuleX DRS 00602 Smart Robot Servo
    /// </summary>
    public class HerkulexDrs0602 : IHerkulexServo
    {
        /// <summary>
        /// Unique Id of the servo in the network. 
        /// </summary>
        public int Id { get; private set;  }
        /// <summary>
        /// Maxmimum possible speed of the servo in seconds per degree
        /// </summary>
        public double MaxSpeed => 0.00274;
        /// <summary>
        /// Maxmimum possible acceleration ratio. 
        /// </summary>
        public int MaxAccRatio => 50;
        /// <summary>
        /// Minimum possible acceleration ratio.
        /// </summary>
        public int MinAccRatio => 0;

        /// <summary>
        /// The neutral position of the servo. Can be between -159 and 159 degrees.
        /// </summary>
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



        /// <summary>
        /// Creates an instance of the HerkuleX DRS 0602 class according to the input parameters
        /// </summary>
        /// <param name="id">Unique id of this servo</param>
        /// <param name="herkulexInterface">The interface which this class should use to communicate with the servo. </param>
        public HerkulexDrs0602(int id, HerkulexInterface herkulexInterface)
        {
            Id = id;
            myHerkulexInterface = herkulexInterface;
            TorqueOn();
            AccelerationRatio(accRatio);
            Thread.Sleep(500);
            TorqueOff();
        }

        /// <summary>
        /// Move the servo to its neutral position. 
        /// </summary>
        public void MoveToNeutralPosition()
        {
            MoveServoPosition(neutralPos, 1000);
        }

        /// <summary>
        /// Enable the torque of the servo
        /// </summary>
        public void TorqueOn()
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() {(int) Torque.RAM, 0x01, (int) Torque.ON};
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));

        }

        /// <summary>
        /// Disable the torque of the servo
        /// </summary>
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

        /// <summary>
        /// Set the color of the LED
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(HerkulexColor color)
        {
            var command = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var finalCommand = command.ConstructSerialProtocol(new List<int>() {53, 0x01, (byte) color});//Address of color is 53
            Send2Servo(finalCommand);
        }

        /// <summary>
        /// Move the servo to a position and waits until it is there.
        /// </summary>
        /// <param name="position">The target position in degrees</param>
        /// <param name="playTime">Amount of time servo should use to reach the target position.</param>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Status of the servo. 
        /// </summary>
        /// <returns>true if there is no error, and false if there is an error.</returns>
        /// <exception cref="Exception">If there was a connection error with the servo.</exception>
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

        /// <summary>
        /// Plays a series of targets.
        /// </summary>
        /// <param name="targets">List with all targets which should be reached</param>
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

        /// <summary>
        /// Reboots the servo. 
        /// </summary>
        public void Reboot()
        {
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.REBOOT_REQ, Id);
            var myCommandHeader = new List<int>();
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
        }

        /// <summary>
        /// Change the communication baud rate of the servo. 
        /// </summary>
        /// <param name="newBaudRate">New baud rate</param>
        public void ChangeBaudRate(HerkulexBaudRate newBaudRate)
        {
            //After u used this method, close the serial port and reopen it with the new baud rate 
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.EEP_WRITE_REQ, Id);
            var myCommandHeader = new List<int>(){(int)Ram.BAUD_RATE_EEP,0x01, (int)newBaudRate };
            Send2Servo(myCommand.ConstructSerialProtocol(myCommandHeader));
            Reboot();
        }

        /// <summary>
        /// Change the unique Id of the servo.
        /// </summary>
        /// <param name="newId">New unique Id</param>
        /// <returns></returns>
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


        /// <summary>
        /// Change the acceleration ratio of the servo. Can be between max- and minAcceleration ratio
        /// </summary>
        /// <param name="newAccRatio">New acceleration ratio</param>
        public void AccelerationRatio(int newAccRatio)
        {
            //higher the ratio, smoother the acceleration 
            if (newAccRatio > MaxAccRatio) this.accRatio = MaxAccRatio;
            else if (newAccRatio < MinAccRatio) this.accRatio = MinAccRatio;
            else this.accRatio = newAccRatio;
            var myCommand = new HerkulexSerialCommand(HerkulexCmd.RAM_WRITE_REQ, Id);
            var myCommandHeader = new List<int>() { (int)Ram.ACCELERATION_RATIO_EEP, 1, accRatio };
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
            // map the command with the answer package according to the manual. 
            var herkulexCmd = Enum.GetValues(typeof(HerkulexCmd)).Cast<HerkulexCmd>();
            var ackRespones = Enum.GetValues(typeof(ACKPackage)).Cast<ACKPackage>();
            var Dict = herkulexCmd.Zip(ackRespones, (k, v) => new { k, v }).ToDictionary(el => el.k, el => el.v);
            return Dict;
        }
    }
}
