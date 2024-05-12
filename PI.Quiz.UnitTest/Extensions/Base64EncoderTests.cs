using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PI.Quiz.Engine.Extensions;
using NUnit.Framework;

namespace PI.Quiz.UnitTest.Extensions
{
    [TestFixture]
    public class Base64EncoderTests
    {
        [Test]
        public void Encode_ReturnEncodeText()
        {
            // Arrange
            string expected = "SGVsbG8gV29ybGQ=";

            // Act
            string result = Base64Encoder.Encode("Hello World");


            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Decode_ReturnDecodeText()
        {
            // Arrange
            string expected = "Hello World";

            // Act
            string result = Base64Encoder.Decode("SGVsbG8gV29ybGQ=");


            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

    }
}
