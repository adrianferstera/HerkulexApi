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
        public void Start(List<double> values, int playTime, List<HerkulexServo> servos, double phaseShift = Math.PI/2)
        {
            var valuesInDegrees = values.Select(el => (maxLimDegrees - minLimDegrees) * el + minLimDegrees).ToList(); 
            var task1 = new Task(() => servos.First().PlaySeries(valuesInDegrees, playTime));
            var task2 = new Task(() => servos.Last().PlaySeries(valuesInDegrees, playTime));
            task1.Start();
            Thread.Sleep(Convert.ToInt32(playTime * phaseShift/Math.PI));
            task2.Start();
            task1.Wait();
            task2.Wait(); 


        }
    }
}
