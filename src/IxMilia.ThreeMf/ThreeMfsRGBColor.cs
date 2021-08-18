using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IxMilia.ThreeMf
{
    public struct ThreeMfsRGBColor : IEquatable<ThreeMfsRGBColor>
    {
        private static Regex ColorPattern = new("^#([0-9A-F]{2}){3,4}$", RegexOptions.IgnoreCase);

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public ThreeMfsRGBColor(byte r, byte g, byte b)
            : this(r, g, b, 255)
        {
        }

        public ThreeMfsRGBColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static bool operator ==(ThreeMfsRGBColor a, ThreeMfsRGBColor b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
        }

        public static bool operator !=(ThreeMfsRGBColor a, ThreeMfsRGBColor b)
        {
            return !(a == b);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is ThreeMfsRGBColor color && Equals(color);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(R | G << 8 | B << 16 ^ A << 24);
        }

        public override readonly string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
        }

        public static ThreeMfsRGBColor Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!ColorPattern.IsMatch(value))
            {
                throw new ThreeMfParseException("Invalid color string.");
            }

            uint u = uint.Parse(value.AsSpan()[1..], NumberStyles.HexNumber);
            if (value.Length == 7)
            {
                // if no alpha specified, assume 0xFF
                u <<= 8;
                u |= 0x000000FF;
            }

            // at this point `u` should be of the format '#RRGGBBAA'
            var r = (u & 0xFF000000) >> 24;
            var g = (u & 0x00FF0000) >> 16;
            var b = (u & 0x0000FF00) >> 8;
            var a = (u & 0x000000FF);

            return new ThreeMfsRGBColor((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public readonly bool Equals(ThreeMfsRGBColor other)
        {
            return this == other;
        }
    }
}
