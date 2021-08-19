using System.Collections.Generic;
using System.IO.Packaging;
using System.Xml.Linq;

namespace IxMilia.ThreeMf
{
    public abstract class ThreeMfResource
    {
        protected const string IdAttributeName = "id";

        public static XName ObjectName { get; } = XName.Get("object", ThreeMfModel.CoreNamespace);
        public static XName BaseMaterialsName { get; } = XName.Get("basematerials", ThreeMfModel.CoreNamespace);
        public static XName ColorGroupName { get; } = XName.Get("colorgroup", ThreeMfModel.MaterialNamespace);
        public static XName Texture2DName { get; } = XName.Get("texture2d", ThreeMfModel.MaterialNamespace);
        public static XName Texture2DGroupName { get; } = XName.Get("texture2dgroup", ThreeMfModel.MaterialNamespace);
        public static XName CompositeMaterialsName { get; } = XName.Get("compositematerials", ThreeMfModel.MaterialNamespace);

        public int Id { get; internal set; }

        public virtual int PropertyCount => 0;

        public abstract XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap);

        public virtual void AfterPartAdded(Package package, PackagePart packagePart)
        {
        }

        public static ThreeMfResource ParseResource(
            XElement element, Dictionary<int, ThreeMfResource> resourceMap, Package package)
        {
            if (element.Name == ObjectName)
            {
                return ThreeMfObject.ParseObject(element, resourceMap, package);
            }
            else if (element.Name == BaseMaterialsName)
            {
                return ThreeMfBaseMaterials.ParseBaseMaterials(element);
            }
            else if (element.Name == ColorGroupName)
            {
                return ThreeMfColorGroup.ParseColorGroup(element);
            }
            else if (element.Name == Texture2DName)
            {
                return ThreeMfTexture2D.ParseTexture(element, package);
            }
            else if (element.Name == Texture2DGroupName)
            {
                return ThreeMfTexture2DGroup.ParseTexture2DGroup(element, resourceMap);
            }
            else if (element.Name == CompositeMaterialsName)
            {
                return ThreeMfCompositeMaterials.ParseCompositeMaterials(element, resourceMap);
            }
            else if (element.Name == SliceStackName)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        public static XName SliceStackName { get; } = XName.Get("slicestack", ThreeMfModel.SliceNamespace);
    }
}
