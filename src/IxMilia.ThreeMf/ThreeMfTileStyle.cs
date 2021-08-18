using System;

namespace IxMilia.ThreeMf
{
    public enum ThreeMfTileStyle
    {
        Wrap,
        Mirror,
        Clamp
    }

    internal static class ThreeMfTileStyleExtensions
    {
        private const string WrapStyle = "wrap";
        private const string MirrorStyle = "mirror";
        private const string ClampStyle = "clamp";

        public static string ToTileStyleString(this ThreeMfTileStyle tileStyle)
        {
            return tileStyle switch
            {
                ThreeMfTileStyle.Wrap => WrapStyle,
                ThreeMfTileStyle.Mirror => MirrorStyle,
                ThreeMfTileStyle.Clamp => ClampStyle,
                _ => throw new InvalidOperationException(),
            };
        }

        public static ThreeMfTileStyle ParseTileStyle(string tileStyle)
        {
            return tileStyle switch
            {
                WrapStyle or null => ThreeMfTileStyle.Wrap,
                MirrorStyle => ThreeMfTileStyle.Mirror,
                ClampStyle => ThreeMfTileStyle.Clamp,
                _ => throw new ThreeMfParseException($"Invalid tile style '{tileStyle}'."),
            };
        }
    }
}
