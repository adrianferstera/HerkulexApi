namespace HerkulexApi
{
    public class HerkulexDatapoint
    {
        public double XValue { get; private set; }
        public double YValue { get; private set; }
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

        public HerkulexDatapoint(double xValue, double yValue)
        {
            this.YValue = yValue;
            this.XValue = xValue; 
        }

        public void ChangeXValue(double x)
        {
            XValue = x; 
        }
       
    }
}
