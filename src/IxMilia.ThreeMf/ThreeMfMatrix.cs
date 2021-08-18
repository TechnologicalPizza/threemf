using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace IxMilia.ThreeMf
{
    public struct ThreeMfMatrix : IEquatable<ThreeMfMatrix>
    {
        internal const string TransformAttributeName = "transform";

        // represents a transform matrix of the form:
        //
        // [ M00 M01 M02 0.0 ]
        // [ M10 M11 M12 0.0 ]
        // [ M20 M21 M22 0.0 ]
        // [ M30 M31 M32 1.0 ]

        public double M00;
        public double M01;
        public double M02;

        public double M10;
        public double M11;
        public double M12;

        public double M20;
        public double M21;
        public double M22;

        public double M30;
        public double M31;
        public double M32;

        public readonly bool IsIdentity =>
            M00 == 1.0 &&
            M01 == 0.0 &&
            M02 == 0.0 &&
            M10 == 0.0 &&
            M11 == 1.0 &&
            M12 == 0.0 &&
            M20 == 0.0 &&
            M21 == 0.0 &&
            M22 == 1.0 &&
            M30 == 0.0 &&
            M31 == 0.0 &&
            M32 == 0.0;

        public ThreeMfMatrix(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22, double m30, double m31, double m32)
            : this()
        {
            M00 = m00;
            M01 = m01;
            M02 = m02;
            M10 = m10;
            M11 = m11;
            M12 = m12;
            M20 = m20;
            M21 = m21;
            M22 = m22;
            M30 = m30;
            M31 = m31;
            M32 = m32;
        }

        public readonly ThreeMfVertex Transform(in ThreeMfVertex v)
        {
            //                       [ M00 M01 M02 0.0 ]
            // [ v.X v.Y v.Z 1.0 ] * [ M10 M11 M12 0.0 ]
            //                       [ M20 M21 M22 0.0 ]
            //                       [ M30 M31 M32 1.0 ]

            var x = v.X * M00 + v.Y * M10 + v.Z * M20 + M30;
            var y = v.Y * M01 + v.Y * M11 + v.Z * M21 + M31;
            var z = v.Z * M02 + v.Y * M12 + v.Z * M22 + M32;
            return new ThreeMfVertex(x, y, z);
        }

        public readonly XAttribute ToXAttribute()
        {
            if (IsIdentity)
            {
                return null;
            }

            var values = new[] { M00, M01, M02, M10, M11, M12, M20, M21, M22, M30, M31, M32 };
            return new XAttribute(TransformAttributeName, string.Join(" ", values));
        }

        public static ThreeMfMatrix ParseMatrix(XAttribute attribute)
        {
            if (attribute == null)
            {
                return Identity;
            }

            var parts = attribute.Value.Split(' ');
            if (parts.Length != 12)
            {
                throw new ThreeMfParseException("Expected 12 parts to parse transform matrix.");
            }

            var values = parts.Select(p => XmlConvert.ToDouble(p)).ToList();
            return new ThreeMfMatrix(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], values[8], values[9], values[10], values[11]);
        }

        public static ThreeMfMatrix Identity => new(1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0);

        public static bool operator ==(in ThreeMfMatrix a, in ThreeMfMatrix b)
        {
            return a.M00 == b.M00
                && a.M01 == b.M01
                && a.M02 == b.M02
                && a.M10 == b.M10
                && a.M11 == b.M11
                && a.M12 == b.M12
                && a.M20 == b.M20
                && a.M21 == b.M21
                && a.M22 == b.M22
                && a.M30 == b.M30
                && a.M31 == b.M31
                && a.M32 == b.M32;
        }

        public static bool operator !=(in ThreeMfMatrix a, in ThreeMfMatrix b)
        {
            return !(a == b);
        }

        public override readonly int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(M00);
            hash.Add(M01);
            hash.Add(M02);
            hash.Add(M10);
            hash.Add(M11);
            hash.Add(M12);
            hash.Add(M20);
            hash.Add(M21);
            hash.Add(M22);
            hash.Add(M30);
            hash.Add(M31);
            hash.Add(M32);
            return hash.ToHashCode();
        }

        public override readonly bool Equals(object obj)
        {
            return obj is ThreeMfMatrix matrix && Equals(matrix);
        }

        public readonly bool Equals(ThreeMfMatrix other)
        {
            return this == other;
        }
    }
}
