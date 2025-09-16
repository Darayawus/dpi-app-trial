using PrintBucket.Models;
using Xunit;

namespace PrintBucket.Tests
{
    public class Class1Tests
    {
        [Fact]
        public void Sum_ReturnsCorrectResult()
        {
            // Arrange
            var class1 = new Class1();
            int a = 2;
            int b = 3;

            // Act
            int result = class1.Sum(a, b);

            // Assert
            Assert.Equal(5, result);
        }
    }
}
