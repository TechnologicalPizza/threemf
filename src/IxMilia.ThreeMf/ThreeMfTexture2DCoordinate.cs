using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public class ThreeMfTexture2DCoordinate : IThreeMfPropertyItem
    {
        private const string UAttributeName = "u";
        private const string VAttributeName = "v";

        public static XName Texture2DCoordinateName { get; } = XName.Get("tex2coord", ThreeMfModel.MaterialNamespace);

        public double U { get; set; }
        public double V { get; set; }

        public ThreeMfTexture2DCoordinate(double u, double v)
        {
            U = u;
            V = v;
        }

        public XElement ToXElement()
        {
            return new XElement(Texture2DCoordinateName,
                new XAttribute(UAttributeName, U),
                new XAttribute(VAttributeName, V));
        }

        public static ThreeMfTexture2DCoordinate ParseCoordinate(XElement element)
        {
            var u = element.AttributeDoubleOrThrow(UAttributeName);
            var v = element.AttributeDoubleOrThrow(VAttributeName);
            return new ThreeMfTexture2DCoordinate(u, v);
        }
    }
}
