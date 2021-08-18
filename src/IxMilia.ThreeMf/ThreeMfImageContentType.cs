using System;

namespace IxMilia.ThreeMf
{
    public enum ThreeMfImageContentType
    {
        Jpeg,
        Png
    }

    internal static class ThreeMfImageContentTypeExtensions
    {
        private const string JpegContentType = "image/jpeg";
        private const string PngContentType = "image/png";

        public static string ToContentTypeString(this ThreeMfImageContentType contentType)
        {
            return contentType switch
            {
                ThreeMfImageContentType.Jpeg => JpegContentType,
                ThreeMfImageContentType.Png => PngContentType,
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToExtensionString(this ThreeMfImageContentType contentType)
        {
            return contentType switch
            {
                ThreeMfImageContentType.Jpeg => ".jpg",
                ThreeMfImageContentType.Png => ".png",
                _ => throw new InvalidOperationException(),
            };
        }

        public static ThreeMfImageContentType ParseContentType(string contentType)
        {
            return contentType switch
            {
                JpegContentType => ThreeMfImageContentType.Jpeg,
                PngContentType => ThreeMfImageContentType.Png,
                _ => throw new ThreeMfParseException($"Invalid image content type '{contentType}'."),
            };
        }
    }
}
