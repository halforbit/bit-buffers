using Lidgren.Network;
using System.Numerics;
using Xunit;

namespace Halforbit.BitBuffers.Tests
{
    public class BitBufferTests
    {
        [Fact]
        public void WriteRead_Enum_BigInteger_Success()
        {
            var output = new BitBuffer();

            output
                .WriteEnum(TestEnumEcho.Bravo, 2)
                .Write((BigInteger)123);

            Assert.Equal(18, output.LengthBits);

            var input = new BitBuffer(output);

            input
                .ReadEnum<TestEnumEcho>(2, out var echo)
                .ReadBigInteger(out var bigInteger);

            Assert.Equal(TestEnumEcho.Bravo, echo);

            Assert.Equal(123, bigInteger);
        }
    }

    public enum TestEnumEcho : byte
    {
        Unknown = 0,

        Alfa,

        Bravo
    }
}
