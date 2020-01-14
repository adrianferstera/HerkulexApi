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
        private IEnumerable<double> PrepareValuesForPlaying(IEnumerable<Datapoint> chartValues, double amplitude, double maxAmplitude, double fc, double period)
        {
            var chartValuesList = chartValues.ToList(); 
            var yPoints = chartValuesList.Select(el => el.yValue).ToList();
            var xPoints = chartValuesList.Select(el => el.xValue).ToList(); 
            var yMax = yPoints.Max();
            var yMin = xPoints.Min();
            var yMaxValueIndex = yPoints.IndexOf(yMax);
            var yMinValueIndex = yPoints.IndexOf(yMin);
            var pointsList = new List<double>();
            if (yMinValueIndex > yMaxValueIndex)
            {
                pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0,yMin));
                pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax,0, yMax));
            }
            else
            {
                pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMax));
                pointsList.Add(Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, yMin));
            }
            var playListServos = new List<double>();
            for (int i = 0; i < period * fc; i++)
            {
                playListServos.AddRange(pointsList);
            }
            var firstValue = Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0,yPoints.First());
            if (Math.Abs(firstValue - playListServos.First()) > 0.01)
            {

                playListServos.Insert(0, firstValue);
            }

            var lastPoint=yPoints.Last(); 
            var lastValue = Map2ServoValue(amplitude, maxAmplitude - amplitude, yMax, 0, lastPoint);
            if (Math.Abs(lastValue - playListServos.Last()) > 0.01)
            {
                playListServos.Add(lastValue);
            }
            //playListServos = playListServos.Select(el => el / 100).ToList();
            var valuesInDegrees = playListServos.Select(el => (maxLimDegrees - minLimDegrees) * el + minLimDegrees).ToList();
            return valuesInDegrees;
        }
        public void StartSeries(IEnumerable<Datapoint> values, double fc, double maxAmplitude, double amplitude, double period, List<HerkulexServo> servos, 
            double phaseShift = Math.PI/2)
        {
            var T = Convert.ToInt32(1 / fc * 1000); //T in ms
            var playTime = T / 2;
            var playValues = PrepareValuesForPlaying(values, amplitude, maxAmplitude, fc, period); 
            var task1 = new Task(() => servos.First().PlaySeries(playValues, playTime));
            var task2 = new Task(() => servos.Last().PlaySeries(playValues, playTime));
            task1.Start();
            Thread.Sleep(Convert.ToInt32(playTime * phaseShift/Math.PI));
            task2.Start();
            task1.Wait();
            task2.Wait(); 
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
            var mappedValue = (yMax - yMin) / (xMax- xMin) * x + yMin;
            return mappedValue;
        }
    }
}
