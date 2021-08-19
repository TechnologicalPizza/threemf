using System;
using System.Xml;
using System.Xml.Linq;

namespace IxMilia.ThreeMf
{
    public struct ThreeMfBoundingBox
    {
        public const string BoundingBoxAttributeName = "box";

        public double U;
        public double V;
        public double Width;
        public double Height;

        public readonly bool IsDefault => U == 0.0 && V == 0.0 && Width == 1.0 && Height == 1.0;

        public ThreeMfBoundingBox(double u, double v, double width, double height)
        {
            U = u;
            V = v;
            Width = width;
            Height = height;
        }

        public static ThreeMfBoundingBox Default => new(0.0, 0.0, 1.0, 1.0);

        public readonly XAttribute ToXAttribute()
        {
            if (IsDefault)
            {
                // default
                return null;
            }

            return new XAttribute(BoundingBoxAttributeName, $"{U} {V} {Width} {Height}");
        }

        public static ThreeMfBoundingBox ParseBoundingBox(string value)
        {
            if (value == null)
            {
                return Default;
            }

            var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                throw new ThreeMfParseException($"Bounding box requires 4 values.");
            }

            try
            {
                double u = XmlConvert.ToDouble(parts[0]);
                double v = XmlConvert.ToDouble(parts[1]);
                double width = XmlConvert.ToDouble(parts[2]);
                double height = XmlConvert.ToDouble(parts[3]);

                return new ThreeMfBoundingBox(u, v, width, height);
            }
            catch (FormatException ex)
            {
                throw new ThreeMfParseException("Invalid value in bounding box.", ex);
            }
        }
    }
}
