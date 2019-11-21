using System.Numerics;
using Xunit;

namespace Halforbit.BitBuffers.Tests
{
    public class BitBufferTests
    {
        [Fact]
        public void WriteRead_Enum_BigInteger_Success()
        {
            var bitWriter = new BitWriter();

            bitWriter
                .WriteEnum(TestEnumEcho.Bravo, 1)
                .Write((short)-123)
                .Write((BigInteger)123);

            Assert.Equal(33, bitWriter.LengthBits);

            var bitReader = new BitReader(bitWriter);

            bitReader
                .ReadEnum<TestEnumEcho>(1, out var echo)
                .ReadInt16(out var shorty)
                .ReadBigInteger(out var bigInteger);

            Assert.Equal(TestEnumEcho.Bravo, echo);

            Assert.Equal(-123, shorty);

            Assert.Equal(123, bigInteger);
        }
    }

    public enum TestEnumEcho : byte
    {
        Alfa = 0,

        Bravo
    }
}
