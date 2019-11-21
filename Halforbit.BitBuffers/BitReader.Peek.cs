using System;

namespace Halforbit.BitBuffers
{
    public partial class BitReader
	{
		/// <summary>
		/// Gets the data buffer
		/// </summary>
		public byte[] PeekDataBuffer() { return _data; }

		//
		// 1 bit
		//
		/// <summary>
		/// Reads a 1-bit Boolean without advancing the read pointer
		/// </summary>
		public bool PeekBoolean()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 1, ReadOverflowError);
			byte retval = BitReaderWriter.ReadByte(_data, 1, _readPosition);
			return (retval > 0 ? true : false);
		}

		//
		// 8 bit 
		//
		/// <summary>
		/// Reads a Byte without advancing the read pointer
		/// </summary>
		public byte PeekByte()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 8, ReadOverflowError);
			byte retval = BitReaderWriter.ReadByte(_data, 8, _readPosition);
			return retval;
		}

		/// <summary>
		/// Reads an SByte without advancing the read pointer
		/// </summary>
		public sbyte PeekSByte()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 8, ReadOverflowError);
			byte retval = BitReaderWriter.ReadByte(_data, 8, _readPosition);
			return (sbyte)retval;
		}

		/// <summary>
		/// Reads the specified number of bits into a Byte without advancing the read pointer
		/// </summary>
		public byte PeekByte(int numberOfBits)
		{
			byte retval = BitReaderWriter.ReadByte(_data, numberOfBits, _readPosition);
			return retval;
		}

		/// <summary>
		/// Reads the specified number of bytes without advancing the read pointer
		/// </summary>
		public byte[] PeekBytes(int numberOfBytes)
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= (numberOfBytes * 8), ReadOverflowError);

			byte[] retval = new byte[numberOfBytes];
			BitReaderWriter.ReadBytes(_data, numberOfBytes, _readPosition, retval, 0);
			return retval;
		}

		/// <summary>
		/// Reads the specified number of bytes without advancing the read pointer
		/// </summary>
		public void PeekBytes(byte[] into, int offset, int numberOfBytes)
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= (numberOfBytes * 8), ReadOverflowError);
			BitBufferException.Assert(offset + numberOfBytes <= into.Length);

			BitReaderWriter.ReadBytes(_data, numberOfBytes, _readPosition, into, offset);
			return;
		}

		//
		// 16 bit
		//
		/// <summary>
		/// Reads an Int16 without advancing the read pointer
		/// </summary>
		public Int16 PeekInt16()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 16, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt16(_data, 16, _readPosition);
			return (short)retval;
		}

		/// <summary>
		/// Reads a UInt16 without advancing the read pointer
		/// </summary>
		public UInt16 PeekUInt16()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 16, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt16(_data, 16, _readPosition);
			return (ushort)retval;
		}

		//
		// 32 bit
		//
		/// <summary>
		/// Reads an Int32 without advancing the read pointer
		/// </summary>
		public Int32 PeekInt32()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 32, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
			return (Int32)retval;
		}

		/// <summary>
		/// Reads the specified number of bits into an Int32 without advancing the read pointer
		/// </summary>
		public Int32 PeekInt32(int numberOfBits)
		{
			BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 32), "ReadInt() can only read between 1 and 32 bits");
			BitBufferException.Assert(_lengthBits - _readPosition >= numberOfBits, ReadOverflowError);

			uint retval = BitReaderWriter.ReadUInt32(_data, numberOfBits, _readPosition);

			if (numberOfBits == 32)
				return (int)retval;

			int signBit = 1 << (numberOfBits - 1);
			if ((retval & signBit) == 0)
				return (int)retval; // positive

			// negative
			unchecked
			{
				uint mask = ((uint)-1) >> (33 - numberOfBits);
				uint tmp = (retval & mask) + 1;
				return -((int)tmp);
			}
		}

		/// <summary>
		/// Reads a UInt32 without advancing the read pointer
		/// </summary>
		public UInt32 PeekUInt32()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 32, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
			return retval;
		}

		/// <summary>
		/// Reads the specified number of bits into a UInt32 without advancing the read pointer
		/// </summary>
		public UInt32 PeekUInt32(int numberOfBits)
		{
			BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 32), "ReadUInt() can only read between 1 and 32 bits");
			//NetException.Assert(m_bitLength - m_readBitPtr >= numberOfBits, "tried to read past buffer size");

			UInt32 retval = BitReaderWriter.ReadUInt32(_data, numberOfBits, _readPosition);
			return retval;
		}

		//
		// 64 bit
		//
		/// <summary>
		/// Reads a UInt64 without advancing the read pointer
		/// </summary>
		public UInt64 PeekUInt64()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 64, ReadOverflowError);

			ulong low = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
			ulong high = BitReaderWriter.ReadUInt32(_data, 32, _readPosition + 32);

			ulong retval = low + (high << 32);

			return retval;
		}

		/// <summary>
		/// Reads an Int64 without advancing the read pointer
		/// </summary>
		public Int64 PeekInt64()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 64, ReadOverflowError);
			unchecked
			{
				ulong retval = PeekUInt64();
				long longRetval = (long)retval;
				return longRetval;
			}
		}

		/// <summary>
		/// Reads the specified number of bits into an UInt64 without advancing the read pointer
		/// </summary>
		public UInt64 PeekUInt64(int numberOfBits)
		{
			BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 64), "ReadUInt() can only read between 1 and 64 bits");
			BitBufferException.Assert(_lengthBits - _readPosition >= numberOfBits, ReadOverflowError);

			ulong retval;
			if (numberOfBits <= 32)
			{
				retval = (ulong)BitReaderWriter.ReadUInt32(_data, numberOfBits, _readPosition);
			}
			else
			{
				retval = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
				retval |= (UInt64)BitReaderWriter.ReadUInt32(_data, numberOfBits - 32, _readPosition + 32) << 32;
			}
			return retval;
		}

		/// <summary>
		/// Reads the specified number of bits into an Int64 without advancing the read pointer
		/// </summary>
		public Int64 PeekInt64(int numberOfBits)
		{
			BitBufferException.Assert(((numberOfBits > 0) && (numberOfBits < 65)), "ReadInt64(bits) can only read between 1 and 64 bits");
			return (long)PeekUInt64(numberOfBits);
		}

		//
		// Floating point
		//
		/// <summary>
		/// Reads a 32-bit Single without advancing the read pointer
		/// </summary>
		public float PeekFloat()
		{
			return PeekSingle();
		}

		/// <summary>
		/// Reads a 32-bit Single without advancing the read pointer
		/// </summary>
		public float PeekSingle()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 32, ReadOverflowError);

			if ((_readPosition & 7) == 0) // read directly
			{
				float retval = BitConverter.ToSingle(_data, _readPosition >> 3);
				return retval;
			}

			byte[] bytes = PeekBytes(4);
			return BitConverter.ToSingle(bytes, 0);
		}

		/// <summary>
		/// Reads a 64-bit Double without advancing the read pointer
		/// </summary>
		public double PeekDouble()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 64, ReadOverflowError);

			if ((_readPosition & 7) == 0) // read directly
			{
				// read directly
				double retval = BitConverter.ToDouble(_data, _readPosition >> 3);
				return retval;
			}

			byte[] bytes = PeekBytes(8);
			return BitConverter.ToDouble(bytes, 0);
		}

		/// <summary>
		/// Reads a string without advancing the read pointer
		/// </summary>
		public string PeekString()
		{
			int wasReadPosition = _readPosition;
			string retval = ReadString();
			_readPosition = wasReadPosition;
			return retval;
		}
	}
}

