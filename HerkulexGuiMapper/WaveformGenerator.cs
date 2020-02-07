using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HerkulexApi;

namespace HerkulexGuiMapper
{
    public class WaveformGenerator
    {
        public static IEnumerable<Datapoint> Generate(WaveformType type, double fc, double period,
            double amplitude, double maxAmplitude)
        {
            var appointList = new List<Datapoint>();
            if (type == WaveformType.NegativeSawtooth)
                appointList = CalculateNegativeSawtoothWave(period, fc, amplitude);
            if (type == WaveformType.PositiveSawtooth)
                appointList = CalculatePositiveSawtoothWave(period, fc, amplitude);
            if (type == WaveformType.Sine)
                appointList = CalculateSineWave(period, fc, amplitude);
            if (type == WaveformType.Triangle)
                appointList = CalculateTriangleWave(period, fc, amplitude);
            if (type == WaveformType.SineTriangle)
                appointList = CalculateSineTriangle(period, fc, amplitude);
            if (type == WaveformType.TriangleSine)
                appointList = CalculateTriangleSine(period, fc, amplitude);

            return appointList;
        }
        public static IEnumerable<Datapoint> GeneratePlayValues(WaveformType type, double fc, double period,
            double amplitude, double maxAmplitude)
        {
            var appointList = new List<Datapoint>();
            if (type == WaveformType.NegativeSawtooth)
                appointList = CalculateNegativeSawtoothWave(period, fc, amplitude, true);
            if (type == WaveformType.PositiveSawtooth)
                appointList = CalculatePositiveSawtoothWave(period, fc, amplitude, true);
            if (type == WaveformType.Sine)
                appointList = CalculateSineWave(period, fc, amplitude, true);
            if (type == WaveformType.Triangle)
                appointList = CalculateTriangleWave(period, fc, amplitude, true);
            if (type == WaveformType.SineTriangle)
                appointList = CalculateSineTriangle(period, fc, amplitude, true);
            if (type == WaveformType.TriangleSine)
                appointList = CalculateTriangleSine(period, fc, amplitude, true);

            return appointList;
        }
        private static List<Datapoint> CalculateTriangleWave(double period, double fc, double amplitude, bool forServo = false)
        {
            var triangleWave = new List<Datapoint>();
            triangleWave.Add(new Datapoint(0, 0));
            if (forServo)
            {
                for (int i = 0; i < period * fc; i++)
                {
                    var gabs = 1.0 / fc;
                    triangleWave.Add(new Datapoint((gabs / 2),
                        amplitude)
                    { AccelerationRatio = 0 });
                    triangleWave.Add(new Datapoint((gabs / 2), 0) { AccelerationRatio = 0 });
                }
                return triangleWave;
            }
            for (int i = 0; i < period * fc; i++)
            {
                var gabs = 1.0 / fc;
                var counterDouble = Convert.ToDouble(i);
                triangleWave.Add(new Datapoint(counterDouble / fc + (gabs / 2),
                    amplitude));
                triangleWave.Add(new Datapoint(counterDouble / fc + gabs, 0));
            }
            return triangleWave;
        }
        private static List<Datapoint> CalculateNegativeSawtoothWave(double period, double fc, double amplitude, bool forServo = false)
        {
            var sawTooths = new List<Datapoint>();
            sawTooths.Add(new Datapoint(0, 0));
            for (int i = 1; i <= period * fc; i++)
            {
                var counterDouble = Convert.ToDouble(i);
                if (forServo)
                {
                    sawTooths.Add(new Datapoint(0, amplitude) { AccelerationRatio = 0 });
                    sawTooths.Add(new Datapoint(1 / fc, 0) { AccelerationRatio = 0 });
                }
                else
                {
                    sawTooths.Add(new Datapoint((counterDouble - 1) / fc, amplitude));
                    sawTooths.Add(new Datapoint(counterDouble / fc, 0));
                }

            }
            return sawTooths;
        }

        private static List<Datapoint> CalculatePositiveSawtoothWave(double period, double fc, double amplitude, bool forServo = false)
        {
            var sawTooths = new List<Datapoint>();
            sawTooths.Add(new Datapoint(0, 0));
            for (int i = 1; i <= period * fc; i++)
            {
                var gabs = 1.0 / fc;
                var counterDouble = Convert.ToDouble(i);
                if (forServo)
                {
                    sawTooths.Add(new Datapoint(1 / fc, amplitude) { AccelerationRatio = 0 });
                    sawTooths.Add(new Datapoint(0, 0) { AccelerationRatio = 0 });
                }
                else
                {
                    sawTooths.Add(new Datapoint(counterDouble / fc, amplitude));
                    sawTooths.Add(new Datapoint(counterDouble / fc, 0));
                }
            }
            return sawTooths;
        }

        private static List<Datapoint> CalculateSineWave(double period, double fc, double amplitude, bool forServo = false)
        {
            if (forServo)
            {
                var sineReplayServo = CalculateTriangleWave(period, fc, amplitude, true);
                return sineReplayServo.Select(el => new Datapoint(el.xValue, el.yValue) { AccelerationRatio = 80 }).ToList();
            }
            var yAxisShift = amplitude / 2;
            var sinData = new List<Datapoint>();
            for (int i = 0; i <= period * 1000; i++)
            {
                var doubleCounter = Convert.ToDouble(i) / 1000;
                sinData.Add(new Datapoint(doubleCounter,
                    ((yAxisShift) * Math.Sin(2 * Math.PI * fc * doubleCounter - Math.PI / 2) + (yAxisShift))));
            }
            return sinData;
        }

        private static List<Datapoint> CalculateSineTriangle(double period, double fc, double amplitude, bool forServo = false)
        {
            if (forServo)
            {
                var sineTriangleReplayServo = CalculateTriangleWave(period, fc, amplitude, true);
                var indexAcceleration = false;
                for (int i = 0; i < sineTriangleReplayServo.Count; i++)
                {
                    if (indexAcceleration)
                    {
                        sineTriangleReplayServo[i].AccelerationRatio = 80;
                        indexAcceleration = false;
                    }
                    else indexAcceleration = true;
                }

                return sineTriangleReplayServo;
            }
            var yAxisShift = amplitude / 2;
            var sinData = new List<Datapoint>();
            var resolution = 100;
            for (int i = 0; i < period * fc; i++)
            {
                var doubleCounter = Convert.ToDouble(i);
                for (int j = 0; j < resolution; j++)
                {
                    sinData.Add(new Datapoint((doubleCounter + 0.5 * Convert.ToDouble(j) / resolution) / fc,
                        ((yAxisShift) * Math.Sin(Math.PI * Convert.ToDouble(j) / resolution - Math.PI / 2) + (yAxisShift))));
                }
                sinData.Add(new Datapoint((i + 1) / fc, 0));

            }
            return sinData;
        }
        private static List<Datapoint> CalculateTriangleSine(double period, double fc, double amplitude, bool forServo = false)
        {
            if (forServo)
            {
                var sineTriangleReplayServo = CalculateTriangleWave(period, fc, amplitude, true);
                var indexAcceleration = true;
                for (int i = 0; i < sineTriangleReplayServo.Count; i++)
                {
                    if (indexAcceleration)
                    {
                        sineTriangleReplayServo[i].AccelerationRatio = 80;
                        indexAcceleration = false;
                    }
                    else indexAcceleration = true;
                }
                return sineTriangleReplayServo;
            }
            var yAxisShift = amplitude / 2;
            var sinData = new List<Datapoint>();
            var resolution = 100;
            sinData.Add(new Datapoint(0, 0));
            for (int i = 0; i < period * fc; i++)
            {
                var doubleCounter = Convert.ToDouble(i);
                sinData.Add(new Datapoint((doubleCounter + 0.5) / fc, amplitude));
                for (int j = 0; j < resolution; j++)
                {
                    sinData.Add(new Datapoint((doubleCounter + 0.5 + 0.5 * Convert.ToDouble(j) / resolution) / fc,
                        ((yAxisShift) * Math.Sin(Math.PI * Convert.ToDouble(j) / resolution - 1.5 * Math.PI) + (yAxisShift))));
                }
            }
            return sinData;
        }

    }
}
