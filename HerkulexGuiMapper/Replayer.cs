using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HerkulexGuiMapper;

namespace HerkulexApi
{
    public class Replayer
    {

        private double minLimDegrees;
        private double maxLimDegrees;


        public Replayer(double minLimit, double maxLimit)
        {
            minLimDegrees = minLimit;
            maxLimDegrees = maxLimit;
        }
        private IEnumerable<Datapoint> PrepareValuesForPlaying(IEnumerable<Datapoint> chartValues, double amplitude, double maxAmplitude, double fc, double period)
        {
            var chartValuesList = chartValues.ToList();
            var yPoints = chartValuesList.Select(el => el.yValue).ToList();
            var xPoints = chartValuesList.Select(el => el.xValue).ToList();
            var yMax = yPoints.Max();
            var yMin = yPoints.Min();
            var yMaxValueIndex = yPoints.IndexOf(yMax);
            var yMinValueIndex = yPoints.IndexOf(yMin);
            var pointsList = new List<Datapoint>();
            if (yMinValueIndex > yMaxValueIndex)
            {
                pointsList.Add(new Datapoint(0,
                    Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMin)));
                pointsList.Add(new Datapoint(0,
                    Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMax)));

                //pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax,0, yMax));
            }
            else
            {
                // var index = yPoints.IndexOf(yMax); new Datapoint(xPoints[yPoints.IndexOf(yMax)]
                pointsList.Add(new Datapoint(0,
                    Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMax)));
                // pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMax));
                //index = yPoints.IndexOf(yMin); new Datapoint(xPoints[yPoints.IndexOf(yMin)]
                pointsList.Add(new Datapoint(0,
                    Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMin)));
                //pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMin));
            }
            var playListServos = new List<Datapoint>();
            for (int i = 0; i < period * fc; i++)
            {
                playListServos.AddRange(pointsList);
            }
            var firstYValue = Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yPoints.First());
            if (Math.Abs(firstYValue - playListServos.First().yValue) > 0.01)
            {

                playListServos.Insert(0, new Datapoint(0, firstYValue));
            }

            var lastPoint = yPoints.Last();
            var lastYValue = Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, lastPoint);
            if (Math.Abs(lastYValue - playListServos.Last().yValue) > 0.01)
            {
                playListServos.Add(new Datapoint(0, lastYValue));
            }
            var valuesInDegreesDatapoint = playListServos.Select(el =>
                new Datapoint(el.xValue, (maxLimDegrees - minLimDegrees) * el.yValue + minLimDegrees)).ToList();
            var finalPlayValues = new List<Datapoint>();
            var gap = fc * period / Convert.ToDouble(valuesInDegreesDatapoint.Count - 1);
            for (int i = 0; i < valuesInDegreesDatapoint.Count; i++)
            {
                var tValue = gap * Convert.ToInt16(i);
                var yValue = valuesInDegreesDatapoint[i].yValue;
                finalPlayValues.Add(new Datapoint(tValue, yValue));
            }
            return finalPlayValues;
        }
        public void StartSeries(WaveformType type, double fc, double maxAmplitude, double amplitude, double period, List<HerkulexServo> servos, int startServo = 1)
        {
            var T = Convert.ToInt32(1 / fc * 1000); //T in ms
            var pauseTimeBetweenServos = T / servos.Count;
            var playValues = WaveformGenerator.GeneratePlayValues(type, fc, period, amplitude, maxAmplitude).ToList();
            //var playValuesForServos = playValues.Select(el =>
            //    new Datapoint(el.xValue * 1000, (maxLimDegrees - minLimDegrees) * el.yValue + minLimDegrees)
            //    { AccelerationRatio = el.AccelerationRatio }).ToList();
            var playValuesForServos = playValues.Select(el =>
                new Datapoint(el.xValue * 1000, Map2ServoValue(maxLimDegrees, minLimDegrees, 1, 0, el.yValue))
                    { AccelerationRatio = el.AccelerationRatio }).ToList();
            
            var replayServoOrder = OrderServoInReplayList(servos, startServo);
            var taskList = new List<List<Task>>();
            var awaitTaskList = new List<Task>();
            foreach (var servoList in replayServoOrder)
            {
                var temporaryTaskList = new List<Task>();
                foreach (var servo in servoList)
                {
                    var task = new Task(() => servo.PlaySeries(playValuesForServos));
                    temporaryTaskList.Add(task);
                    awaitTaskList.Add(task);
                }
                taskList.Add(temporaryTaskList);
            }

            foreach (var servoPairTask in taskList)
            {
                foreach (var servoTask in servoPairTask)
                {
                    servoTask.Start();
                }
                //Thread.Sleep(pauseTimeBetweenServos);
               // Thread.Sleep(100);
            }

            Task.WaitAll(awaitTaskList.ToArray());
        }

        public void Move2Position(double value, List<HerkulexServo> servos)
        {
           // var valueInDeg = (maxLimDegrees - minLimDegrees) * value + minLimDegrees;
            var valueInDeg = Map2ServoValue(maxLimDegrees, minLimDegrees, 1, 0, value); 
            var taskList = new List<Task>();
            foreach (var servo in servos)
            {
                var task = new Task(() => servo.MoveServoPosition(valueInDeg, 1000));
                taskList.Add(task);
            }

            foreach (var task in taskList)
            {
                task.Start(); 
            }
            Task.WaitAll(taskList.ToArray()); 
        }

        private static double Map2ServoValue(double yMax, double yMin, double xMax, double xMin, double x)
        {
            var mappedValue = (yMax - yMin) / (xMax - xMin) * x + yMin;
            return mappedValue;
        }

        private List<List<HerkulexServo>> OrderServoInReplayList(List<HerkulexServo> myServos, int startValue)
        {
            var replayList = new List<List<HerkulexServo>>();
            replayList.Add(myServos.Where(el => el.Id == startValue).ToList());
            /*
            replayList.Add(myServos.Where(el => el.Id == startValue + 1 || el.Id == startValue - 1).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 2 || el.Id == startValue - 2).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 3 || el.Id == startValue - 3).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 4 || el.Id == startValue - 4).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 5 || el.Id == startValue - 5).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 6 || el.Id == startValue - 6).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 7 || el.Id == startValue - 7).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 8 || el.Id == startValue - 8).ToList());
            replayList.Add(myServos.Where(el => el.Id == startValue + 9 || el.Id == startValue - 9).ToList());
             return replayList.Where(el => el.Count > 0).ToList();*/

            return addServoRecursive(myServos, replayList, startValue + 1, startValue - 1);
        }
        private List<List<HerkulexServo>> addServoRecursive(List<HerkulexServo> myServos, List<List<HerkulexServo>> replayServoList, int servoId1, int servoId2)
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
