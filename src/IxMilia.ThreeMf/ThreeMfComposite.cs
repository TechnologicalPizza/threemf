using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public readonly struct ThreeMfComposite
    {
        public const string ValuesAttributeName = "values";

        public static XName CompositeName { get; } = XName.Get("composite", ThreeMfModel.MaterialNamespace);

        public double[] Values { get; }

        public ThreeMfComposite(double[] values)
        {
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public XElement ToXElement()
        {
            return new XElement(CompositeName,
                new XAttribute(ValuesAttributeName, string.Join(' ', Values)));
        }

        public static ThreeMfComposite ParseComposite(XElement element)
        {
            var value = element.AttributeOrThrow(ValuesAttributeName).Value;
            var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var values = parts.Select(x => XmlConvert.ToDouble(x));
            return new ThreeMfComposite(values.ToArray());
        }
    }
}
