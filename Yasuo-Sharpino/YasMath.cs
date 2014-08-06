using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace Yasuo_Sharpino
{
    class YasMath
    {
        public static bool interact(Vector2 p1, Vector2 p2, Vector2 pC, float radius)
        {
            Vector2 p3 = new Vector2();
            p3.X = pC.X + radius;
            p3.Y = pC.Y + radius;
           // if ((p2.X - p1.X) == 0)
              //  Console.WriteLine("whawdawdawdawdawfawfqwdfawfawfawfaw");
            float m = ((p2.Y - p1.Y) / (p2.X - p1.X));
            float Constant = (m * p1.X) - p1.Y;

            float b = -(2f * ((m * Constant) + p3.X + (m * p3.Y)));
            float a = (1 + (m * m));
            float c = ((p3.X * p3.X) + (p3.Y * p3.Y) - (radius * radius) + (2f * Constant * p3.Y) + (Constant * Constant));
            float D = ((b * b) - (4f * a * c));
            if (D > 0)
            {
              //  Console.WriteLine("Interacted!");
                return true;
            }
            else
                return false;

        }

        public static float DistanceFromPointToLine(Vector2 l1, Vector2 l2, Vector2 point)
        {
            return Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
                    (float)Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }

    }
}
