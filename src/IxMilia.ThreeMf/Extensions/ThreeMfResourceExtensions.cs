using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace IxMilia.ThreeMf.Extensions
{
    internal static class ThreeMfResourceExtensions
    {
        public static bool TryGetPropertyResource<TResource>(
            this Dictionary<int, ThreeMfResource> resourceMap,
            XElement element, 
            string attributeName, 
            [MaybeNullWhen(false)] out TResource propertyResource)
            where TResource : ThreeMfResource
        {
            var propertyReferenceAttribute = element.Attribute(attributeName);
            if (propertyReferenceAttribute == null)
            {
                propertyResource = default;
                return false;
            }

            if (!int.TryParse(propertyReferenceAttribute.Value, out var propertyIndex))
            {
                throw new ThreeMfParseException(
                    $"Property reference index '{propertyReferenceAttribute.Value}' is not an int.");
            }

            if (!resourceMap.TryGetValue(propertyIndex, out ThreeMfResource resource))
            {
                propertyResource = default;
                return false;
            }

            if (resource is not TResource typedResource)
            {
                throw new ThreeMfParseException(
                    $"Property resource was expected to be of type {typeof(TResource)}.");
            }

            propertyResource = typedResource;
            return true;
        }

        public static int ParseAndValidateRequiredResourceIndex(
            this ThreeMfResource propertyResource, XElement element, string attributeName)
        {
            var index = element.AttributeIntOrThrow(attributeName);
            propertyResource.ValidatePropertyIndex(index);
            return index;
        }

        public static int ParseAndValidateOptionalResourceIndex(
            this ThreeMfResource propertyResource, XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                return -1;
            }

            if (!int.TryParse(attribute.Value, out var index))
            {
                throw new ThreeMfParseException($"Property index '{attribute.Value}' is not an int.");
            }

            propertyResource.ValidatePropertyIndex(index);

            return index;
        }

        private static void ValidatePropertyIndex(this ThreeMfResource propertyResource, int index)
        {
            int propertyCount = propertyResource.PropertyCount;

            if (index < 0 || index >= propertyCount)
            {
                throw new ThreeMfParseException(
                    $"Property index is out of range.  Value must be [0, {propertyCount}).");
            }
        }
    }
}
