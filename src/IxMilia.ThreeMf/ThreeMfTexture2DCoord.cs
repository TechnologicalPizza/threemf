using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public readonly struct ThreeMfTexture2DCoord
    {
        private const string UAttributeName = "u";
        private const string VAttributeName = "v";

        public static XName Texture2DCoordName { get; } = XName.Get("tex2coord", ThreeMfModel.MaterialNamespace);

        public double U { get; }
        public double V { get; }

        public ThreeMfTexture2DCoord(double u, double v)
        {
            U = u;
            V = v;
        }

        public XElement ToXElement()
        {
            return new XElement(Texture2DCoordName,
                new XAttribute(UAttributeName, U),
                new XAttribute(VAttributeName, V));
        }

        public static ThreeMfTexture2DCoord ParseCoordinate(XElement element)
        {
            var u = element.AttributeDoubleOrThrow(UAttributeName);
            var v = element.AttributeDoubleOrThrow(VAttributeName);
            return new ThreeMfTexture2DCoord(u, v);
        }
    }
}
