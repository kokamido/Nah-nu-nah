using System;
using System.Collections.Generic;
using System.Drawing;

namespace lib
{
    public class Arrow : IDrawable
    {
        private List<ColoredPoint> points;

        public Arrow(Tuple<int,int> start, int length, double angle, Color color)
        {
            
        }

        public Arrow(Tuple<int, int> start, Tuple<int, int> end)
        {
            var stick = MathHelper.GetLineSegment(start, end);
            
        }
        
        public bool DrawOn(Bitmap bitmap, Tuple<int,int> point)
        {
            throw new System.NotImplementedException();
        }
    }
}