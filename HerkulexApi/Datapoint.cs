using System.Threading;

namespace HerkulexApi
{
    public class Datapoint: HerkulexApi.IValue
    {
        public double xValue { get; private set; }
        public double yValue { get; private set; }
        private int accelerationRatio =-1; 
        public int AccelerationRatio
        {
            get=>accelerationRatio;
            set
            {
                if (value > 80)
                {
                    accelerationRatio = 80;
                }
                else if (value < 0)
                {
                    accelerationRatio = 0;
                }
                else accelerationRatio = value;
            }
        }

        public Datapoint(double xValue, double yValue)
        {
            this.yValue = yValue;
            this.xValue = xValue; 
        }

        public void ChangeXValue(double x)
        {
            xValue = x; 
        }
       
    }
}
