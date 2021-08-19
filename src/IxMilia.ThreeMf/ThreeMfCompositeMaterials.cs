using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using IxMilia.ThreeMf.Collections;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public class ThreeMfCompositeMaterials : ThreeMfResource
    {
        public const string MatIdAttributeName = "matid";
        public const string MatIndicesAttributeName = "matindices";
        public const string DisplayPropertiesIdAttributeName = "displaypropertiesid";

        public ListNonNull<ThreeMfComposite> Composites { get; } = new ListNonNull<ThreeMfComposite>();
        public ThreeMfBaseMaterials BaseMaterials { get; set; }
        public ThreeMfBase[] Materials { get; set; }
        public ThreeMfDisplayProperties DisplayProperties { get; set; }

        public override int PropertyCount => Composites.Count;

        public override XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap)
        {
            return new XElement(Texture2DGroupName,
                new XAttribute(IdAttributeName, Id),
                new XAttribute(MatIdAttributeName, resourceMap[BaseMaterials]),
                new XAttribute(MatIndicesAttributeName, string.Join(' ', Materials.Select(x => BaseMaterials.Bases.IndexOf(x)))),
                Composites.Select(c => c.ToXElement()));
        }

        public static ThreeMfCompositeMaterials ParseCompositeMaterials(
            XElement element, Dictionary<int, ThreeMfResource> resourceMap)
        {
            var matIndicesValue = element.AttributeOrThrow(MatIndicesAttributeName).Value;
            var matIndicesParts = matIndicesValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var matIndices = matIndicesParts.Select(x => XmlConvert.ToInt32(x)).ToArray();

            var compositeMaterials = new ThreeMfCompositeMaterials();
            compositeMaterials.Id = element.AttributeIntOrThrow(IdAttributeName);

            if (resourceMap.TryGetPropertyResource<ThreeMfBaseMaterials>(
                element, MatIdAttributeName, out var baseMaterials))
            {
                compositeMaterials.BaseMaterials = baseMaterials;
            }

            compositeMaterials.Materials = new ThreeMfBase[matIndices.Length];
            for (int i = 0; i < matIndices.Length; i++)
            {
                compositeMaterials.Materials[i] = compositeMaterials.BaseMaterials.Bases[matIndices[i]];
            }

            if (resourceMap.TryGetPropertyResource<ThreeMfDisplayProperties>(
                element, DisplayPropertiesIdAttributeName, out var displayproperties))
            {
                compositeMaterials.DisplayProperties = displayproperties;
            }

            foreach (var compositeElement in element.Elements(ThreeMfComposite.CompositeName))
            {
                var composite = ThreeMfComposite.ParseComposite(compositeElement);
                compositeMaterials.Composites.Add(composite);
            }

            return compositeMaterials;
        }
    }
}
