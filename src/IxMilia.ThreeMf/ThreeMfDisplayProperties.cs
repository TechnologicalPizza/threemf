using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace IxMilia.ThreeMf
{
    public class ThreeMfDisplayProperties : ThreeMfResource
    {
        public override int PropertyCount => throw new NotImplementedException();

        public override XElement ToXElement(Dictionary<ThreeMfResource, int> resourceMap)
        {
            throw new NotImplementedException();
        }
    }
}
