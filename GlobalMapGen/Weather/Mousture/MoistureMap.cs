using System.Drawing;
using System.Runtime.Serialization;
using lib;

namespace GlobalMap.Weather.Mousture
{
    [KnownType(typeof(DefaultMoisture))]
    [DataContract]
    public abstract class MoistureMap :IDrawable
    {
        public double[,] Map;
        [DataMember(EmitDefaultValue = false)] protected double[] map;

        protected Color DryColor = Color.Yellow;
        protected Color MoistColor = Color.ForestGreen;


        public void DrawOn(Bitmap bitmap)
        {
            Map.ForEach((val,x,y)=> bitmap.SetPixel(x,y,GetColor(bitmap.GetPixel(x,y),x,y)));
        }

        protected abstract Color GetColor(Color bitmapPixel, int x, int y);

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            Map = map.ToDoubleDimension(Map.GetLength(0), Map.GetLength(1));
        }

        [OnSerializing]
        protected void OnSerializing(StreamingContext context)
        {
            map = Map.ToSingleDimension();
        }
    }

    public class DefaultMoisture : MoistureMap
    {
        public DefaultMoisture(Map map)
        {

        }

        private double[,] GetInitialMoisture(Map map)
        {
            return null;
        }

        protected override Color GetColor(Color bitmapPixel, int x, int y)
        {
            throw new System.NotImplementedException();
        }
    }
}