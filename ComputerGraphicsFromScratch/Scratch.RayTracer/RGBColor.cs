using Kz.Engine.DataStructures;
using Raylib_cs;

namespace Scratch.RayTracer
{
    public struct RGBColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A => 255;

        public RGBColor()
        {
            R = 0;
            G = 0;
            B = 0;
        }

        public RGBColor(byte r, byte g, byte b)
        {
            R = (byte)Clamp(r);
            G = (byte)Clamp(g);
            B = (byte)Clamp(b);
        }

        private static int Clamp(int value)
        {
            if (value < 0) return 0;
            else if (value > 255) return 255;
            return value;
        }

        public static RGBColor Add(RGBColor lhs, RGBColor rhs)
        {
            return new RGBColor(
                (byte)Clamp(lhs.R + rhs.R),
                (byte)Clamp(lhs.G + rhs.G),
                (byte)Clamp(lhs.B + rhs.B));
        }

        public static RGBColor operator +(RGBColor lhs, RGBColor rhs) => Add(lhs, rhs);

        public static RGBColor Multiply(float lhs, RGBColor rhs)
        {
            return new RGBColor(
                (byte)Clamp((int)(lhs * rhs.R)),
                (byte)Clamp((int)(lhs * rhs.G)),
                (byte)Clamp((int)(lhs * rhs.B)));
        }
        
        public static RGBColor operator *(float lhs, RGBColor rhs) => Multiply(lhs, rhs);
        public static RGBColor operator *(RGBColor lhs, float rhs) => Multiply(rhs, lhs);

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public override string ToString()
        {
            return $"R: {R}, G: {G}, B: {B}";
        }        
    }
}