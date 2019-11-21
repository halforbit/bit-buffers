
namespace Halforbit.BitBuffers
{
    public abstract class BitBuffer
	{
		protected byte[] _data;
		
        protected int _lengthBits;

		/// <summary>
		/// Gets the length of the used portion of the buffer in bytes
		/// </summary>
		public int LengthBytes => (_lengthBits + 7) >> 3;

		/// <summary>
		/// Gets the length of the used portion of the buffer in bits
		/// </summary>
		public int LengthBits => _lengthBits;
	}
}
