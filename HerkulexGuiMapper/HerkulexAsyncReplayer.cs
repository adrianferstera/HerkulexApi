using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HerkulexGuiMapper;

namespace HerkulexApi
{
    public class HerkulexAsyncReplayer
    {

        private double minLimDegrees;
        private double maxLimDegrees;


        public HerkulexAsyncReplayer(double minLimit, double maxLimit)
        {
            minLimDegrees = minLimit;
            maxLimDegrees = maxLimit;
        }
       public void StartSeries(WaveformType type, double fc, double maxAmplitude, double amplitude, double period, List<IHerkulexServo> servos, bool spatialOnlyPattern = false, int startServo = 1)
        {
            var T = Convert.ToInt32(1 / fc * 1000); //T in ms
            var pauseTimeBetweenServos = 0;
            if (spatialOnlyPattern) pauseTimeBetweenServos = T / servos.Count;

            var playValues = WaveformGenerator.GeneratePlayValues(type, fc, period, amplitude, maxAmplitude).ToList();
            var playValuesForServos = playValues.Select(el =>
                new HerkulexDatapoint(el.XValue * 1000, Map2ServoValue(maxLimDegrees, minLimDegrees, 1, 0, el.YValue))
                    { AccelerationRatio = el.AccelerationRatio }).ToList();
            
            var replayServoOrder = OrderServoInReplayList(servos, startServo);
            var threadList = new List<List<Thread>>();
            var awaitThreadList = new List<Thread>();
            foreach (var servoList in replayServoOrder)
            {
                var temporaryThreadList = new List<Thread>();
                foreach (var servo in servoList)
                {
                    var thread = new Thread(() => servo.PlaySeries(playValuesForServos));
                    temporaryThreadList.Add(thread);
                    awaitThreadList.Add(thread);
                }
                threadList.Add(temporaryThreadList);
            }

            foreach (var servoPairThread in threadList)
            {
                foreach (var servoThread in servoPairThread)
                {
                    servoThread.Start();
                }
                Thread.Sleep(pauseTimeBetweenServos);
            }

            foreach (var thread in awaitThreadList)
            {
                //Wait until the threads are finished by joining another to it, its similar to Task.Waitall....
                thread.Join(); 
            }
        }

        public void Move2Position(double value, List<IHerkulexServo> servos)
        {
            var valueInDeg = Map2ServoValue(maxLimDegrees, minLimDegrees, 1, 0, value); 
            var threadList = new List<Thread>();
            foreach (var servo in servos)
            {
                var task = new Thread(() => servo.MoveServoPosition(valueInDeg, 1000));
                threadList.Add(task);
            }
            foreach (var thread in threadList) thread.Start();
            foreach (var thread in threadList) thread.Join(); 
        }

        private static double Map2ServoValue(double yMax, double yMin, double xMax, double xMin, double x)
        {
            var mappedValue = (yMax - yMin) / (xMax - xMin) * x + yMin;
            return mappedValue;
        }

        private List<List<IHerkulexServo>> OrderServoInReplayList(List<IHerkulexServo> myServos, int startValue)
        {
            var replayList = new List<List<IHerkulexServo>>();
            replayList.Add(myServos.Where(el => el.Id == startValue).ToList());
            return addServoRecursive(myServos, replayList, startValue + 1, startValue - 1);
        }
        private List<List<IHerkulexServo>> addServoRecursive(List<IHerkulexServo> myServos, List<List<IHerkulexServo>> replayServoList, int servoId1, int servoId2)
        {
            var temporaryServoList = myServos.Where(el => el.Id == servoId1 || el.Id == servoId2).ToList();
            replayServoList.Add(temporaryServoList);
            if (servoId1 +1 <= 8 || servoId2-1 >= 1)
            {
                addServoRecursive(myServos, replayServoList, servoId1 + 1, servoId2 - 1);
            }
            return replayServoList;
        }
    }
}
