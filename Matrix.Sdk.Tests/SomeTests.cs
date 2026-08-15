namespace Matrix.Sdk.Tests
{
    using System;
    using Matrix.Sdk.Core.Infrastructure.Dto.Event;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    [TestClass]
    public class SomeTests
    {
        [TestMethod]
        public void GifInfoComesFromImageData()
        {
            var bytes = new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a', 0x40, 0x01, 0xf0, 0x00 };

            var info = ImageInfo.FromBytes("animation.GIF", bytes);

            Assert.AreEqual("image/gif", info.mimetype);
            Assert.AreEqual(bytes.Length, info.size);
            Assert.AreEqual(320, info.w);
            Assert.AreEqual(240, info.h);
        }

        [TestMethod]
        public void PngInfoComesFromImageData()
        {
            var bytes = new byte[24];
            new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }.CopyTo(bytes, 0);
            bytes[18] = 0x02;
            bytes[19] = 0x80;
            bytes[22] = 0x01;
            bytes[23] = 0xe0;

            var info = ImageInfo.FromBytes("image.png", bytes);

            Assert.AreEqual("image/png", info.mimetype);
            Assert.AreEqual(640, info.w);
            Assert.AreEqual(480, info.h);
        }

        [TestMethod]
        public void ImageEventSerializesMatrixInfo()
        {
            var request = new ImageMessageEventRequest(
                "animation.gif",
                "mxc://example/id",
                new ImageInfo(240, 320, "image/gif", 1234));

            var json = JsonConvert.SerializeObject(request);

            StringAssert.Contains(json, "\"mimetype\":\"image/gif\"");
            StringAssert.Contains(json, "\"size\":1234");
            StringAssert.Contains(json, "\"w\":320");
            StringAssert.Contains(json, "\"h\":240");
        }

        [TestMethod]
        public void ImageExtensionMustMatchData()
        {
            var bytes = new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a', 1, 0, 1, 0 };

            Assert.ThrowsException<ArgumentException>(() => ImageInfo.FromBytes("not-a-gif.png", bytes));
        }
    }
}
