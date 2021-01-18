using System;

namespace Zodiark.Namazu
{
    public class Point
    {
        public float X, Y, Z;

        public Point(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static float GetAngle(Point a, Point b)
        {
            float x = b.X - a.X;
            float y = b.Z - a.Z;
            return (float)Math.Atan2(y, x);
        }

        public static Point Midpoint(Point a, Point b)
        {
            float x = (a.X + b.X) / 2;
            float y = (a.Y + b.Y) / 2;
            float z = (a.Z + b.Z) / 2;
            return new Point(x, y, z);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}