using System;
using System.Globalization;

namespace UberStrok.Core.Common
{
    public struct Color
    {
        public Color(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            A = 1;
        }

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public float R;
        public float G;
        public float B;
        public float A;

        public override bool Equals(object obj)
        {
            return obj is Color && this == (Color)obj;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }

        public static Color Convert(string hex)
        {
            System.Drawing.Color col; // from System.Drawing or System.Windows.Media
            if (hex.Length == 6)
                col = System.Drawing.Color.FromArgb(255, // hardcoded opaque
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber));
            else if (hex.Length == 8)
                col = System.Drawing.Color.FromArgb(
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
            else
                throw new ArgumentException("Hex invalid");
            return new Color(col.R, col.G, col.B, col.A);
        }
    }
}
