using System;

namespace Halforbit.BitBuffers
{
    public partial class BitBuffer
	{
		byte[] _data;
		
        int _lengthBits;
		
        int _readPosition;

        public BitBuffer(byte[] data = default)
        {
            if (data != default)
            {
                _data = data;

                _lengthBits = data.Length * 8;
            }
        }

		/// <summary>
		/// Gets the length of the used portion of the buffer in bytes
		/// </summary>
		public int LengthBytes => (_lengthBits + 7) >> 3;

		/// <summary>
		/// Gets the length of the used portion of the buffer in bits
		/// </summary>
		public int LengthBits => _lengthBits;

		/// <summary>
		/// Gets the read position in the buffer, in bits (not bytes)
		/// </summary>
		public long Position => _readPosition;

		/// <summary>
		/// Gets the position in the buffer in bytes; note that the bits of the first returned byte may already have been read - check the Position property to make sure.
		/// </summary>
		public int PositionInBytes => (_readPosition / 8);

        public byte[] ToArray()
        {
            var lengthBytes = LengthBytes;

            var bytes = new byte[lengthBytes];

            Array.Copy(_data, 0, bytes, 0, lengthBytes);

            return bytes;
        }

        public static implicit operator byte[](BitBuffer bitBuffer) => bitBuffer.ToArray();

        public static implicit operator BitBuffer(byte[] bytes) => new BitBuffer(bytes);
	}
}
