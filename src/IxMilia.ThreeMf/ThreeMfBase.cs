using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public readonly struct ThreeMfBase
    {
        private const string NameAttributeName = "name";
        private const string DisplayColorAttributeName = "displaycolor";

        public static XName BaseName { get; } = XName.Get("base", ThreeMfModel.CoreNamespace);

        public string Name { get; }
        public ThreeMfsRGBColor Color { get; }

        public ThreeMfBase(string name, ThreeMfsRGBColor color)
        {
            Name = name;
            Color = color;
        }

        public XElement ToXElement()
        {
            return new XElement(BaseName,
                new XAttribute(NameAttributeName, Name),
                new XAttribute(DisplayColorAttributeName, Color.ToString()));
        }

        public static ThreeMfBase ParseBaseMaterial(XElement baseElement)
        {
            XAttribute name = baseElement.AttributeOrThrow(NameAttributeName);
            var color = ThreeMfsRGBColor.Parse(baseElement.AttributeOrThrow(DisplayColorAttributeName).Value);
            return new ThreeMfBase(name.Value, color);
        }
    }
}
