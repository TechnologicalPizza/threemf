using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IxMilia.ThreeMf.Collections;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public class ThreeMfColorGroup : ThreeMfResource, IThreeMfPropertyResource
    {
        public ListNonNull<ThreeMfColor> Colors { get; } = new ListNonNull<ThreeMfColor>();

        IEnumerable<IThreeMfPropertyItem> IThreeMfPropertyResource.PropertyItems => Colors;

        public override XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap)
        {
            return new XElement(ColorGroupName,
                new XAttribute(IdAttributeName, Id),
                Colors.Select(c => c.ToXElement()));
        }

        public static ThreeMfColorGroup ParseColorGroup(XElement element)
        {
            var colorGroup = new ThreeMfColorGroup();
            colorGroup.Id = element.AttributeIntOrThrow(IdAttributeName);
            foreach (var colorElement in element.Elements(ThreeMfColor.ColorName))
            {
                var color = ThreeMfColor.ParseColor(colorElement);
                colorGroup.Colors.Add(color);
            }

            return colorGroup;
        }
    }
}
