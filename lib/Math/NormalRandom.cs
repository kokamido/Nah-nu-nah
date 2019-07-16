using System;

namespace lib
{
    public class NormalRandom
    {
        private readonly Random _randi;
        private readonly double m;
        private readonly double t;
        private double? _buffer;

        public NormalRandom(double m, double t, int? seed = null)
        {
            _randi = seed.HasValue? new Random(seed.Value) : new Random();
            this.m = m;
            this.t = t;
            _buffer = null;
        }

        public double Next()
        {
            if (_buffer.HasValue)
            {
                var rs = _buffer.Value;
                _buffer = null;
                return rs;
            }
            var (z1, z2) = GetNormalRandom(_randi.NextDouble(), _randi.NextDouble());
            _buffer = m + t * z2;
            return m + t * z1;

        }

        private (double, double) GetNormalRandom(double r, double t)
        {
            return (Math.Sqrt(-2*Math.Log(r)) * Math.Sin(2*Math.PI*t), Math.Sqrt(-2*Math.Log(r)) * Math.Cos(2*Math.PI*t));
        }
    }
}