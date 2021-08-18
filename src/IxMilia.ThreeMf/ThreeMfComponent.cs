using System;
using System.Collections.Generic;
using System.Xml.Linq;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public class ThreeMfComponent
    {
        private const string ObjectIdAttributeName = "objectid";

        public static XName ComponentName { get; } = XName.Get("component", ThreeMfModel.ModelNamespace);

        private ThreeMfResource _obj;

        public ThreeMfResource Object
        {
            get => _obj;
            set => _obj = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ThreeMfMatrix Transform { get; set; }

        public ThreeMfComponent(ThreeMfResource obj, ThreeMfMatrix transform)
        {
            Object = obj;
            Transform = transform;
        }

        public XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap)
        {
            var objectId = resourceMap[Object];
            return new XElement(ComponentName,
                new XAttribute(ObjectIdAttributeName, objectId),
                Transform.ToXAttribute());
        }

        public static ThreeMfComponent ParseComponent(XElement element, Dictionary<int, ThreeMfResource> resourceMap)
        {
            if (element == null)
            {
                return null;
            }

            if (!int.TryParse(element.AttributeOrThrow(ObjectIdAttributeName).Value, out int objectId) &&
                !resourceMap.ContainsKey(objectId))
            {
                throw new ThreeMfParseException($"Invalid object id {objectId}.");
            }

            var obj = resourceMap[objectId];
            var transform = ThreeMfMatrix.ParseMatrix(element.Attribute(ThreeMfMatrix.TransformAttributeName));
            return new ThreeMfComponent(obj, transform);
        }
    }
}
