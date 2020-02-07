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



            //playListServos = playListServos.Select(el => el / 100).ToList();
            // var valuesInDegrees = playListServos.Select(el => (maxLimDegrees - minLimDegrees) * el.yValue + minLimDegrees);
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
        public void StartSeries(WaveformType type, double fc, double maxAmplitude, double amplitude, double period, List<HerkulexServo> servos)
        {
            var T = Convert.ToInt32(1 / fc * 1000); //T in ms
            var pauseTimeBetweenServos = T / servos.Count;
            var playValues = WaveformGenerator.GeneratePlayValues(type, fc, period, amplitude, maxAmplitude).ToList();
            var playValuesForServos = playValues.Select(el =>
                new Datapoint(el.xValue * 1000, (maxLimDegrees - minLimDegrees) * el.yValue + minLimDegrees)
                { AccelerationRatio = el.AccelerationRatio }).ToList();

            var taskList = new List<Task>();
            foreach (var servo in servos)
            {
                var task = new Task(() => servo.PlaySeries(playValuesForServos));
                taskList.Add(task);
            }

            /* //taskList[0].Start();
             //Thread.Sleep(playTime);
             taskList[1].Start();
             Thread.Sleep(pauseTimeBetweenServos);
             taskList[2].Start();
             Thread.Sleep(pauseTimeBetweenServos);
             taskList[3].Start();*/
            //Thread.Sleep(playTime);
            //taskList[4].Start();
            foreach (var task in taskList)
            {
                task.Start();
                Thread.Sleep(pauseTimeBetweenServos);
            }

            Task.WaitAll(taskList.ToArray());
        }

        public void Move2Position(double value, List<HerkulexServo> servos)
        {
            var valueInDeg = (maxLimDegrees - minLimDegrees) * value + minLimDegrees;
            var task1 = new Task(() => servos.First().MoveServoPosition(valueInDeg, 1000));
            var task2 = new Task(() => servos.Last().MoveServoPosition(valueInDeg, 1000));
            task1.Start();
            task2.Start();
            task1.Wait();
            task2.Wait();

        }

        private static double Map2ServoValue(double yMax, double yMin, double xMax, double xMin, double x)
        {
            var mappedValue = (yMax - yMin) / (xMax - xMin) * x + yMin;
            return mappedValue;
        }
    }
}
