using PrintBucket.Graphics;
using Xunit;
using Serilog;

namespace PrintBucket.Tests
{
    public class ImageProcessorTests
    {
        [Fact]
        public void NetVips_ShouldBeAvailable()
        {
            // Act
            ImageProcessor.Initialization();
            bool isAvailable = ImageProcessor.IsNetVipsAvailable();

            // Assert
            Assert.True(isAvailable, "NetVips should be available");
        }

        [Fact]
        public void NetVips_ShouldReturnVersion()
        {
            // Act
            ImageProcessor.Initialization();
            string version = ImageProcessor.GetNetVipsVersion();

            // Assert
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            Log.Information("NetVips version: {Version}", version);
        }
    }
}