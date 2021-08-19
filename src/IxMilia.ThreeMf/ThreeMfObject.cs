using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Xml.Linq;
using IxMilia.ThreeMf.Collections;
using IxMilia.ThreeMf.Extensions;

namespace IxMilia.ThreeMf
{
    public class ThreeMfObject : ThreeMfResource
    {
        protected const string NameAttributeName = "name";
        protected const string PartNumberAttributeName = "partnumber";
        protected const string PropertyReferenceAttributeName = "pid";
        protected const string PropertyIndexAttributeName = "pindex";
        protected const string ThumbnailAttributeName = "thumbnail";
        protected const string TypeAttributeName = "type";
        protected const string ThumbnailRelationshipType = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail";

        public const string ThumbnailPathPrefix = "/3D/Thumbnails/";

        public static XName MeshName { get; } = XName.Get("mesh", ThreeMfModel.CoreNamespace);
        public static XName ComponentsName { get; } = XName.Get("components", ThreeMfModel.CoreNamespace);

        public ThreeMfObjectType Type { get; set; }
        public ThreeMfResource PropertyResource { get; set; }
        public int PropertyIndex { get; set; }
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public ThreeMfImageContentType ThumbnailContentType { get; set; }
        public byte[] ThumbnailData { get; set; }

        private ThreeMfMesh _mesh;
        private Uri _thumbnailUri;

        public ThreeMfMesh Mesh
        {
            get => _mesh;
            set => _mesh = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ListNonNull<ThreeMfComponent> Components { get; } = new ListNonNull<ThreeMfComponent>();

        public ThreeMfObject()
        {
            Type = ThreeMfObjectType.Model;
            Mesh = new ThreeMfMesh();
        }

        public override XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap)
        {
            string thumbnailPath = null;
            if (ThumbnailData != null)
            {
                thumbnailPath = string.Concat(ThumbnailPathPrefix, Guid.NewGuid().ToString("N"), ThumbnailContentType.ToExtensionString());
                _thumbnailUri = new Uri(thumbnailPath, UriKind.RelativeOrAbsolute);
            }
            else
            {
                _thumbnailUri = null;
            }

            return new XElement(ObjectName,
                new XAttribute(IdAttributeName, Id),
                new XAttribute(TypeAttributeName, Type.ToString().ToLowerInvariant()),
                PropertyResource == null
                    ? null
                    : new[]
                    {
                        new XAttribute(PropertyReferenceAttributeName, resourceMap[(ThreeMfResource)PropertyResource]),
                        new XAttribute(PropertyIndexAttributeName, PropertyIndex)
                    },
                thumbnailPath == null ? null : new XAttribute(ThumbnailAttributeName, thumbnailPath),
                PartNumber == null ? null : new XAttribute(PartNumberAttributeName, PartNumber),
                Name == null ? null : new XAttribute(NameAttributeName, Name),
                Mesh.ToXElement(resourceMap),
                Components.Count == 0 ? null : new XElement(ComponentsName, Components.Select(c => c.ToXElement(resourceMap))));
        }

        public override void AfterPartAdded(Package package, PackagePart packagePart)
        {
            if (_thumbnailUri != null)
            {
                package.WriteBinary(_thumbnailUri.ToString(), ThumbnailContentType.ToContentTypeString(), ThumbnailData);
                packagePart.CreateRelationship(_thumbnailUri, TargetMode.Internal, ThumbnailRelationshipType);
            }
        }

        public static ThreeMfObject ParseObject(
            XElement element, Dictionary<int, ThreeMfResource> resourceMap, Package package)
        {
            var obj = new ThreeMfObject();
            obj.Id = element.AttributeIntOrThrow(IdAttributeName);
            obj.Type = ParseObjectType(element.Attribute(TypeAttributeName)?.Value);
            obj.PartNumber = element.Attribute(PartNumberAttributeName)?.Value;
            obj.Name = element.Attribute(NameAttributeName)?.Value;

            var meshElement = element.Element(MeshName);
            if (meshElement != null)
            {
                obj.Mesh = ThreeMfMesh.ParseMesh(meshElement, resourceMap);
            }

            var thumbnailPath = element.Attribute(ThumbnailAttributeName)?.Value;
            if (thumbnailPath != null)
            {
                obj.ThumbnailData = package.GetPartBytes(thumbnailPath);
            }

            var components = element.Element(ComponentsName);
            if (components != null)
            {
                foreach (var componentElement in components.Elements())
                {
                    var component = ThreeMfComponent.ParseComponent(componentElement, resourceMap);
                    obj.Components.Add(component);
                }
            }

            if (resourceMap.TryGetPropertyResource<ThreeMfResource>(
                element, PropertyReferenceAttributeName, out var propertyResource))
            {
                obj.PropertyResource = propertyResource;
                obj.PropertyIndex = propertyResource.ParseAndValidateRequiredResourceIndex(element, PropertyIndexAttributeName);
            }
            else if (element.Attribute(PropertyReferenceAttributeName) == null && element.Attribute(PropertyIndexAttributeName) != null)
            {
                throw new ThreeMfParseException($"Attribute '{PropertyIndexAttributeName}' is only valid if '{PropertyReferenceAttributeName}' is also specified.");
            }

            return obj;
        }

        public static ThreeMfObjectType ParseObjectType(string value)
        {
            return value switch
            {
                "model" or null => ThreeMfObjectType.Model,
                "support" => ThreeMfObjectType.Support,
                "other" => ThreeMfObjectType.Other,
                _ => throw new ThreeMfParseException($"Invalid object type '{value}'."),
            };
        }
    }
}
