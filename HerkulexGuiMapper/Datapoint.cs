using System;
using System.Collections.Generic;
using System.Text;

namespace HerkulexGuiMapper
{
    public class Datapoint: HerkulexApi.IValue
    {
        public double xValue { get; }
        public double yValue { get; }
        public Datapoint(double xValue, double yValue)
        {
            this.yValue = yValue;
            this.xValue = xValue; 
        }

       
    }
}
