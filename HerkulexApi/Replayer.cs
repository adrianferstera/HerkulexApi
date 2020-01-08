using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public void Start(List<double> values, int playTime, List<HerkulexServo> servos)
        {
            /*foreach (var servo in servos)
            {
                servo.MoveToNeutralPosition();
            }*/

            var valuesInDegrees = values.Select(el => (maxLimDegrees - minLimDegrees) * el + minLimDegrees).ToList(); 
            //var valueInDegrees = (maxLimDegrees - minLimDegrees) * value + minLimDegrees;
            var task1 = new Task(() => servos.First().PlaySeries(valuesInDegrees, playTime));
            var task2 = new Task(() => servos.Last().PlaySeries(valuesInDegrees, playTime));
            task1.Start();
            Thread.Sleep(Convert.ToInt32(playTime * 0.5));
            task2.Start();
            task1.Wait();
            task2.Wait(); 


        }
    }
}
