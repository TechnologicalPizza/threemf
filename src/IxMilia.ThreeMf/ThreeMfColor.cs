using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public readonly struct ThreeMfColor
    {
        private const string ColorAttributeName = "color";

        public static XName ColorName { get; } = XName.Get("color", ThreeMfModel.MaterialNamespace);

        public ThreeMfsRGBColor Color { get; }

        public ThreeMfColor(ThreeMfsRGBColor color)
        {
            Color = color;
        }

        public XElement ToXElement()
        {
            return new XElement(ColorName,
                new XAttribute(ColorAttributeName, Color.ToString()));
        }

        public static ThreeMfColor ParseColor(XElement element)
        {
            var color = ThreeMfsRGBColor.Parse(element.AttributeOrThrow(ColorAttributeName).Value);
            return new ThreeMfColor(color);
        }
    }
}
