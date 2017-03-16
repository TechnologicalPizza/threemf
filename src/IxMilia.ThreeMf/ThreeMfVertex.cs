﻿// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Xml.Linq;

namespace IxMilia.ThreeMf
{
    public struct ThreeMfVertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public ThreeMfVertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public static bool operator ==(ThreeMfVertex a, ThreeMfVertex b)
        {
            if (ReferenceEquals(a, b))
                return true;
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(ThreeMfVertex a, ThreeMfVertex b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ThreeMfVertex)
                return this == (ThreeMfVertex)obj;
            return false;
        }

        internal static ThreeMfVertex ParseVertex(XElement element)
        {
            var x = ParseDouble(element, "x");
            var y = ParseDouble(element, "y");
            var z = ParseDouble(element, "z");
            return new ThreeMfVertex(x, y, z);
        }

        private static double ParseDouble(XElement element, string attributeName)
        {
            var att = element.Attribute(attributeName);
            if (att == null)
            {
                throw new ThreeMfParseException($"Missing required attribute '{attributeName}'.");
            }

            if (!double.TryParse(att.Value, out var value))
            {
                throw new ThreeMfParseException($"Unable to parse '{att.Value}' as a double.");
            }

            return value;
        }
    }
}
