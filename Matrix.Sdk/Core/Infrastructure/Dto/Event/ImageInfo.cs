using System;
using System.IO;

namespace Matrix.Sdk.Core.Infrastructure.Dto.Event
{
    public record ImageInfo(int h, int w, string mimetype, int size)
    {
        public static ImageInfo FromBytes(string filename, byte[] imageData)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("An image filename is required.", nameof(filename));
            if (imageData == null)
                throw new ArgumentNullException(nameof(imageData));

            var extension = Path.GetExtension(filename).ToLowerInvariant();

            if (IsGif(imageData))
            {
                RequireExtension(extension, ".gif", filename);
                var width = imageData[6] | imageData[7] << 8;
                var height = imageData[8] | imageData[9] << 8;
                return Create(width, height, "image/gif", imageData.Length);
            }

            if (IsPng(imageData))
            {
                RequireExtension(extension, ".png", filename);
                var width = ReadBigEndianInt32(imageData, 16);
                var height = ReadBigEndianInt32(imageData, 20);
                return Create(width, height, "image/png", imageData.Length);
            }

            throw new NotSupportedException($"Unsupported image data: {filename}");
        }

        private static ImageInfo Create(int width, int height, string mimetype, int size)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Image dimensions must be positive.");
            return new ImageInfo(height, width, mimetype, size);
        }

        private static bool IsGif(byte[] data)
        {
            if (data.Length < 10) return false;
            return data[0] == 'G' && data[1] == 'I' && data[2] == 'F' && data[3] == '8' &&
                   (data[4] == '7' || data[4] == '9') && data[5] == 'a';
        }

        private static bool IsPng(byte[] data)
        {
            if (data.Length < 24) return false;
            byte[] signature = { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
            for (var i = 0; i < signature.Length; i++)
                if (data[i] != signature[i]) return false;
            return true;
        }

        private static int ReadBigEndianInt32(byte[] data, int offset)
        {
            return data[offset] << 24 | data[offset + 1] << 16 | data[offset + 2] << 8 | data[offset + 3];
        }

        private static void RequireExtension(string actual, string expected, string filename)
        {
            if (actual != expected)
                throw new ArgumentException($"Image data does not match its filename extension: {filename}", nameof(filename));
        }
    }
}
