using System;
using System.Linq;
using System.Runtime.Serialization;
using lib;

namespace GlobalMap.Weather.Wind
{
    public static class CycloneMetaSettings
    {
        public const int CyclonePowerToRangeMultiplier = 23;
        public const int LandscapeRepulsionRadius = 10;
        public const double LandscapeTailForWindDecrease = 75;
        public const double HeightPointRepulsionAngle = Math.PI/200;
        public const double HeightPointRepulsionPowerDecrease = 0.35;
        public const double CycloneEigenvalueFromPowerMultiplier = 1/20.0;
    }

    [DataContract]
    public class Cyclone
    {
        [DataMember(EmitDefaultValue = false)] public Tuple<int, int> Eye;
        [DataMember(EmitDefaultValue = false)] public double Power;
        public double Range => Power * CycloneMetaSettings.CyclonePowerToRangeMultiplier;
        [DataMember(EmitDefaultValue = false)] public bool IsAntiCyclone;
        [DataMember(EmitDefaultValue = false)] public bool IsCounterClockwise;

        [DataMember(EmitDefaultValue = false)] private static int _ahahaMagicMantiss = 5;
        [DataMember(EmitDefaultValue = false)] private static int _ahahaMagicExponent = 100;
        private static int _ahahaMagicNumber => _ahahaMagicMantiss * _ahahaMagicExponent;

        private static double[] preCalculatedSigmoid = Enumerable.Range(0, _ahahaMagicNumber)
            .Select(x => -(Sigmoid(- 2.5 + x / (double) _ahahaMagicExponent)*0.53-0.4929)).ToArray();

        private static double Sigmoid(double x) => x / Math.Sqrt(1 + x * x);

        public Tuple<double, double> GetVecIn(Tuple<int, int> point)
        {
            var range = MathHelper.GetEuclideanRange(point, Eye);
            if (!IsCounterClockwise)
            {
                var buf = point.Minus(Eye);
                buf = (-buf.Item1).MakeTupleWith(buf.Item2);
                point = buf.Plus(Eye);
            }

            var res = range > Range
                ? 0.0.MakeTupleWith(0.0)
                : (IsAntiCyclone
                    ? GetAntiCycloneVec(point)
                    : GetCycloneVec(point)).Get1Vec()
                .Multiply(preCalculatedSigmoid[Math.Min(_ahahaMagicNumber - 1, (range / Range * _ahahaMagicNumber).Round())] * Power);

            return IsCounterClockwise ? res : (-res.Item1).MakeTupleWith(res.Item2);
        }

        private Tuple<double, double> GetCycloneVec(Tuple<int, int> coords)
        {
            var p = coords.Minus(Eye);
            var x = p.Item1;
            var y = p.Item2;
            var power = (Power * CycloneMetaSettings.CycloneEigenvalueFromPowerMultiplier - 1) / 2.0;
            return (-x + 2 * y).MakeTupleWith(-5 * x - power * y);
        }

        private Tuple<double, double> GetAntiCycloneVec(Tuple<int, int> coords)
        {
            var p = coords.Minus(Eye);
            var power = Power * CycloneMetaSettings.CycloneEigenvalueFromPowerMultiplier;
            var x = p.Item1;
            var y = p.Item2;
            return (power * x + y).MakeTupleWith(-x + power * y);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            preCalculatedSigmoid = Enumerable.Range(0, _ahahaMagicNumber)
                .Select(x => Math.Tanh(_ahahaMagicMantiss - x / (double) _ahahaMagicExponent)).ToArray();
        }
    }
}