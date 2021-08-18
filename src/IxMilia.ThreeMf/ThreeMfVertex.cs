using System;
using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public struct ThreeMfVertex : IEquatable<ThreeMfVertex>
    {
        private const string XAttributeName = "x";
        private const string YAttributeName = "y";
        private const string ZAttributeName = "z";

        internal static XName VertexName = XName.Get("vertex", ThreeMfModel.ModelNamespace);

        public double X;
        public double Y;
        public double Z;

        public ThreeMfVertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public readonly XElement ToXElement()
        {
            return new XElement(VertexName,
                new XAttribute(XAttributeName, X),
                new XAttribute(YAttributeName, Y),
                new XAttribute(ZAttributeName, Z));
        }

        public override readonly string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public readonly bool Equals(ThreeMfVertex other)
        {
            return this == other;
        }

        public static bool operator ==(ThreeMfVertex a, ThreeMfVertex b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(ThreeMfVertex a, ThreeMfVertex b)
        {
            return !(a == b);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is ThreeMfVertex vertex && Equals(vertex);
        }

        public static ThreeMfVertex ParseVertex(XElement element)
        {
            double x = element.AttributeDoubleOrThrow(XAttributeName);
            double y = element.AttributeDoubleOrThrow(YAttributeName);
            double z = element.AttributeDoubleOrThrow(ZAttributeName);
            return new ThreeMfVertex(x, y, z);
        }
    }
}
