using System;
using System.Xml.Linq;

namespace IxMilia.ThreeMf.Extensions
{
    internal static class XmlExtensions
    {
        public static XAttribute AttributeOrThrow(this XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                throw new ThreeMfParseException($"Expected attribute '{attributeName}'.");
            }
            return attribute;
        }

        public static int AttributeIntOrThrow(this XElement element, string attributeName)
        {
            XAttribute attribute = element.AttributeOrThrow(attributeName);
            try
            {
                return (int)attribute;
            }
            catch (Exception ex)
            {
                throw new ThreeMfParseException(
                    $"Unable to parse attribute '{attributeName}' as an int.", ex);
            }
        }

        public static double AttributeDoubleOrThrow(this XElement element, string attributeName)
        {
            XAttribute attribute = element.AttributeOrThrow(attributeName);
            try
            {
                return (double)attribute;
            }
            catch (Exception ex)
            {
                throw new ThreeMfParseException(
                    $"Unable to parse attribute '{attributeName}' as a double.", ex);
            }
        }
    }
}
