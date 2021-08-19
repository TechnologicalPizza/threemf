using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IxMilia.ThreeMf.Collections;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public class ThreeMfTexture2DGroup : ThreeMfResource
    {
        private const string TextureIdAttributeName = "texid";

        public ListNonNull<ThreeMfTexture2DCoord> Coordinates { get; } = new ListNonNull<ThreeMfTexture2DCoord>();
        public ThreeMfTexture2D Texture { get; set; }

        public override int PropertyCount => Coordinates.Count;

        public ThreeMfTexture2DGroup(ThreeMfTexture2D texture)
        {
            Texture = texture;
        }

        public override XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap)
        {
            return new XElement(Texture2DGroupName,
                new XAttribute(IdAttributeName, Id),
                new XAttribute(TextureIdAttributeName, resourceMap[Texture]),
                Coordinates.Select(c => c.ToXElement()));
        }

        public static ThreeMfTexture2DGroup ParseTexture2DGroup(
            XElement element, Dictionary<int, ThreeMfResource> resourceMap)
        {
            var texture = resourceMap[element.AttributeIntOrThrow(TextureIdAttributeName)] as ThreeMfTexture2D;
            var textureGroup = new ThreeMfTexture2DGroup(texture);
            textureGroup.Id = element.AttributeIntOrThrow(IdAttributeName);
            foreach (var textureCoordinateElement in element.Elements(ThreeMfTexture2DCoord.Texture2DCoordName))
            {
                var coord = ThreeMfTexture2DCoord.ParseCoordinate(textureCoordinateElement);
                textureGroup.Coordinates.Add(coord);
            }

            return textureGroup;
        }
    }
}
