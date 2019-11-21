using System;
using System.Text;
using System.Threading;

namespace Halforbit.BitBuffers
{
    /// <summary>
    /// Base class for NetIncomingMessage and NetOutgoingMessage
    /// </summary>
    public partial class BitReader : BitBuffer
	{
		const string ReadOverflowError = "Trying to read past the buffer size - likely caused by mismatching Write/Reads, different size or order.";
		
        const int BufferSize = 64; // Min 8 to hold anything but strings. Increase it if readed strings usally don't fit inside the buffer
		
        static object _buffer;

        protected int _readPosition;

        public BitReader(byte[] data)
        {
            _data = data;

            _lengthBits = data.Length * 8;
        }

        /// <summary>
        /// Gets the read position in the buffer, in bits (not bytes)
        /// </summary>
        public long Position => _readPosition;

        /// <summary>
        /// Gets the position in the buffer in bytes; note that the bits of the first returned byte may already have been read - check the Position property to make sure.
        /// </summary>
        public int PositionInBytes => (_readPosition / 8);

        public static implicit operator BitReader(byte[] bytes) => new BitReader(bytes);

        /// <summary>
        /// Reads a boolean value (stored as a single bit) written using Write(bool)
        /// </summary>
        public bool ReadBoolean()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 1, ReadOverflowError);
			byte retval = BitReaderWriter.ReadByte(_data, 1, _readPosition);
			_readPosition += 1;
			return (retval > 0 ? true : false);
		}
		
        public BitReader ReadBoolean(out bool value)
        {
            value = ReadBoolean();

            return this;
        }

		/// <summary>
		/// Reads a byte
		/// </summary>
		public byte ReadByte()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 8, ReadOverflowError);
			byte retval = BitReaderWriter.ReadByte(_data, 8, _readPosition);
			_readPosition += 8;
			return retval;
		}

        public BitReader ReadByte(out byte value)
        {
            value = ReadByte();

            return this;
        }

		/// <summary>
		/// Reads a signed byte
		/// </summary>
		public sbyte ReadSByte()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 8, ReadOverflowError);
			byte retval = BitReaderWriter.ReadByte(_data, 8, _readPosition);
			_readPosition += 8;
			return (sbyte)retval;
		}

        public BitReader ReadSByte(out sbyte value)
        {
            value = ReadSByte();

            return this;
        }

        /// <summary>
        /// Reads 1 to 8 bits into a byte
        /// </summary>
        public byte ReadByte(int numberOfBits)
		{
			BitBufferException.Assert(numberOfBits > 0 && numberOfBits <= 8, "ReadByte(bits) can only read between 1 and 8 bits");
			byte retval = BitReaderWriter.ReadByte(_data, numberOfBits, _readPosition);
			_readPosition += numberOfBits;
			return retval;
		}

        public BitReader ReadByte(int numberOfBits, out byte value)
        {
            value = ReadByte(numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads the specified number of bytes
        /// </summary>
        public byte[] ReadBytes(int numberOfBytes)
		{
			BitBufferException.Assert(_lengthBits - _readPosition + 7 >= (numberOfBytes * 8), ReadOverflowError);

			byte[] retval = new byte[numberOfBytes];
			BitReaderWriter.ReadBytes(_data, numberOfBytes, _readPosition, retval, 0);
			_readPosition += (8 * numberOfBytes);
			return retval;
		}

        public BitReader Read(int numberOfBytes, out byte[] value)
        {
            value = ReadBytes(numberOfBytes);

            return this;
        }

		/// <summary>
		/// Reads the specified number of bytes into a preallocated array
		/// </summary>
		/// <param name="into">The destination array</param>
		/// <param name="offset">The offset where to start writing in the destination array</param>
		/// <param name="numberOfBytes">The number of bytes to read</param>
		public BitReader ReadBytes(byte[] into, int offset, int numberOfBytes)
		{
			BitBufferException.Assert(_lengthBits - _readPosition + 7 >= (numberOfBytes * 8), ReadOverflowError);
			BitBufferException.Assert(offset + numberOfBytes <= into.Length);

			BitReaderWriter.ReadBytes(_data, numberOfBytes, _readPosition, into, offset);
			_readPosition += (8 * numberOfBytes);
			
            return this;
		}

		/// <summary>
		/// Reads the specified number of bits into a preallocated array
		/// </summary>
		/// <param name="into">The destination array</param>
		/// <param name="offset">The offset where to start writing in the destination array</param>
		/// <param name="numberOfBits">The number of bits to read</param>
		public BitReader ReadBits(byte[] into, int offset, int numberOfBits)
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= numberOfBits, ReadOverflowError);
			BitBufferException.Assert(offset + BitUtility.BytesToHoldBits(numberOfBits) <= into.Length);

			int numberOfWholeBytes = numberOfBits / 8;
			int extraBits = numberOfBits - (numberOfWholeBytes * 8);

			BitReaderWriter.ReadBytes(_data, numberOfWholeBytes, _readPosition, into, offset);
			_readPosition += (8 * numberOfWholeBytes);

			if (extraBits > 0)
				into[offset + numberOfWholeBytes] = ReadByte(extraBits);

			return this;
		}

		/// <summary>
		/// Reads a 16 bit signed integer written using Write(Int16)
		/// </summary>
		public Int16 ReadInt16()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 16, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt16(_data, 16, _readPosition);
			_readPosition += 16;
			return (short)retval;
		}

        public BitReader ReadInt16(out Int16 value)
        {
            value = ReadInt16();

            return this;
        }

        /// <summary>
        /// Reads a 16 bit unsigned integer written using Write(UInt16)
        /// </summary>
        public UInt16 ReadUInt16()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 16, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt16(_data, 16, _readPosition);
			_readPosition += 16;
			return (ushort)retval;
		}

        public BitReader ReadUInt16(out UInt16 value)
        {
            value = ReadUInt16();

            return this;
        }

        /// <summary>
        /// Reads a 32 bit signed integer written using Write(Int32)
        /// </summary>
        public Int32 ReadInt32()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 32, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
			_readPosition += 32;
			return (Int32)retval;
		}

        public BitReader ReadInt32(out Int32 value)
        {
            value = ReadInt32();

            return this;
        }

        /// <summary>
        /// Reads a signed integer stored in 1 to 32 bits, written using Write(Int32, Int32)
        /// </summary>
        public Int32 ReadInt32(int numberOfBits)
		{
			BitBufferException.Assert(numberOfBits > 0 && numberOfBits <= 32, "ReadInt32(bits) can only read between 1 and 32 bits");
			BitBufferException.Assert(_lengthBits - _readPosition >= numberOfBits, ReadOverflowError);

			uint retval = BitReaderWriter.ReadUInt32(_data, numberOfBits, _readPosition);
			_readPosition += numberOfBits;

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

        public BitReader ReadInt32(int numberOfBits, out Int32 value)
        {
            value = ReadInt32(numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads an 32 bit unsigned integer written using Write(UInt32)
        /// </summary>
        public UInt32 ReadUInt32()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 32, ReadOverflowError);
			uint retval = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
			_readPosition += 32;
			return retval;
		}

        public BitReader ReadUInt32(out UInt32 value)
        {
            value = ReadUInt32();

            return this;
        }

        /// <summary>
        /// Reads an unsigned integer stored in 1 to 32 bits, written using Write(UInt32, Int32)
        /// </summary>
        public UInt32 ReadUInt32(int numberOfBits)
		{
			BitBufferException.Assert(numberOfBits > 0 && numberOfBits <= 32, "ReadUInt32(bits) can only read between 1 and 32 bits");
			//NetException.Assert(m_bitLength - m_readBitPtr >= numberOfBits, "tried to read past buffer size");

			UInt32 retval = BitReaderWriter.ReadUInt32(_data, numberOfBits, _readPosition);
			_readPosition += numberOfBits;
			return retval;
		}

        public BitReader Read(int numberOfBits, out UInt32 value)
        {
            value = ReadUInt32(numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads a 64 bit unsigned integer written using Write(UInt64)
        /// </summary>
        public UInt64 ReadUInt64()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 64, ReadOverflowError);

			ulong low = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);
			_readPosition += 32;
			ulong high = BitReaderWriter.ReadUInt32(_data, 32, _readPosition);

			ulong retval = low + (high << 32);

			_readPosition += 32;
			return retval;
		}

        public BitReader Read(out UInt64 value)
        {
            value = ReadUInt64();

            return this;
        }

        /// <summary>
        /// Reads a 64 bit signed integer written using Write(Int64)
        /// </summary>
        public Int64 ReadInt64()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 64, ReadOverflowError);
			unchecked
			{
				ulong retval = ReadUInt64();
				long longRetval = (long)retval;
				return longRetval;
			}
		}

        public BitReader ReadInt64(out Int64 value)
        {
            value = ReadInt64();

            return this;
        }

        /// <summary>
        /// Reads an unsigned integer stored in 1 to 64 bits, written using Write(UInt64, Int32)
        /// </summary>
        public UInt64 ReadUInt64(int numberOfBits)
		{
			BitBufferException.Assert(numberOfBits > 0 && numberOfBits <= 64, "ReadUInt64(bits) can only read between 1 and 64 bits");
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
			_readPosition += numberOfBits;
			return retval;
		}

        public BitReader ReadUInt64(out UInt64 value)
        {
            value = ReadUInt64();

            return this;
        }

        /// <summary>
        /// Reads a signed integer stored in 1 to 64 bits, written using Write(Int64, Int32)
        /// </summary>
        public Int64 ReadInt64(int numberOfBits)
		{
			BitBufferException.Assert(((numberOfBits > 0) && (numberOfBits <= 64)), "ReadInt64(bits) can only read between 1 and 64 bits");
			return (long)ReadUInt64(numberOfBits);
		}

        public BitReader ReadInt64(int numberOfBits, out Int64 value)
        {
            value = ReadInt64(numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads a 32 bit floating point value written using Write(Single)
        /// </summary>
        public float ReadFloat()
		{
			return ReadSingle();
		}

        public BitReader ReadFloat(out float value)
        {
            value = ReadFloat();

            return this;
        }

        /// <summary>
        /// Reads a 32 bit floating point value written using Write(Single)
        /// </summary>
        public float ReadSingle()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 32, ReadOverflowError);

			if ((_readPosition & 7) == 0) // read directly
			{
				float retval = BitConverter.ToSingle(_data, _readPosition >> 3);
				_readPosition += 32;
				return retval;
			}

			byte[] bytes = (byte[]) Interlocked.Exchange(ref _buffer, null) ?? new byte[BufferSize];
			ReadBytes(bytes, 0, 4);
			float res = BitConverter.ToSingle(bytes, 0);
			_buffer = bytes;
			return res;
		}

        public BitReader ReadSingle(out float value)
        {
            value = ReadSingle();

            return this;
        }

        /// <summary>
        /// Reads a 64 bit floating point value written using Write(Double)
        /// </summary>
        public double ReadDouble()
		{
			BitBufferException.Assert(_lengthBits - _readPosition >= 64, ReadOverflowError);

			if ((_readPosition & 7) == 0) // read directly
			{
				// read directly
				double retval = BitConverter.ToDouble(_data, _readPosition >> 3);
				_readPosition += 64;
				return retval;
			}

			byte[] bytes = (byte[]) Interlocked.Exchange(ref _buffer, null) ?? new byte[BufferSize];
			ReadBytes(bytes, 0, 8);
			double res = BitConverter.ToDouble(bytes, 0);
			_buffer = bytes;
			return res;
		}

        public BitReader ReadDouble(out double value)
        {
            value = ReadDouble();

            return this;
        }

        //
        // Variable bit count
        //

        /// <summary>
        /// Reads a variable sized UInt32 written using WriteVariableUInt32()
        /// </summary>
        public uint ReadVariableUInt32()
		{
			int num1 = 0;
			int num2 = 0;
			while (_lengthBits - _readPosition >= 8)
			{
				byte num3 = this.ReadByte();
				num1 |= (num3 & 0x7f) << num2;
				num2 += 7;
				if ((num3 & 0x80) == 0)
					return (uint)num1;
			}

			// ouch; failed to find enough bytes; malformed variable length number?
			return (uint)num1;
		}

        public BitReader ReadVariableUInt32(out uint value)
        {
            value = ReadVariableUInt32();

            return this;
        }

        /// <summary>
        /// Reads a variable sized Int32 written using WriteVariableInt32()
        /// </summary>
        public int ReadVariableInt32()
		{
			uint n = ReadVariableUInt32();
			return (int)(n >> 1) ^ -(int)(n & 1); // decode zigzag
		}

        public BitReader ReadVariableInt32(out int value)
        {
            value = ReadVariableInt32();

            return this;
        }

        /// <summary>
        /// Reads a variable sized Int64 written using WriteVariableInt64()
        /// </summary>
        public Int64 ReadVariableInt64()
		{
			UInt64 n = ReadVariableUInt64();
			return (Int64)(n >> 1) ^ -(long)(n & 1); // decode zigzag
		}

        public BitReader ReadVariableInt64(out Int64 value)
        {
            value = ReadInt64();

            return this;
        }

        /// <summary>
        /// Reads a variable sized UInt32 written using WriteVariableInt64()
        /// </summary>
        public UInt64 ReadVariableUInt64()
		{
			UInt64 num1 = 0;
			int num2 = 0;
			while (_lengthBits - _readPosition >= 8)
			{
				//if (num2 == 0x23)
				//	throw new FormatException("Bad 7-bit encoded integer");

				byte num3 = this.ReadByte();
				num1 |= ((UInt64)num3 & 0x7f) << num2;
				num2 += 7;
				if ((num3 & 0x80) == 0)
					return num1;
			}

			// ouch; failed to find enough bytes; malformed variable length number?
			return num1;
		}

        public BitReader ReadVariableUInt64(out UInt64 value)
        {
            value = ReadUInt64();

            return this;
        }

        /// <summary>
        /// Reads a 32 bit floating point value written using WriteSignedSingle()
        /// </summary>
        /// <param name="numberOfBits">The number of bits used when writing the value</param>
        /// <returns>A floating point value larger or equal to -1 and smaller or equal to 1</returns>
        public float ReadSignedSingle(int numberOfBits)
		{
			uint encodedVal = ReadUInt32(numberOfBits);
			int maxVal = (1 << numberOfBits) - 1;
			return ((float)(encodedVal + 1) / (float)(maxVal + 1) - 0.5f) * 2.0f;
		}

        public BitReader ReadSignedSingle(int numberOfBits, out float value)
        {
            value = ReadSignedSingle(numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads a 32 bit floating point value written using WriteUnitSingle()
        /// </summary>
        /// <param name="numberOfBits">The number of bits used when writing the value</param>
        /// <returns>A floating point value larger or equal to 0 and smaller or equal to 1</returns>
        public float ReadUnitSingle(int numberOfBits)
		{
			uint encodedVal = ReadUInt32(numberOfBits);
			int maxVal = (1 << numberOfBits) - 1;
			return (float)(encodedVal + 1) / (float)(maxVal + 1);
		}

        public BitReader ReadUnitSingle(int numberOfBits, out float value)
        {
            value = ReadUnitSingle(numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads a 32 bit floating point value written using WriteRangedSingle()
        /// </summary>
        /// <param name="min">The minimum value used when writing the value</param>
        /// <param name="max">The maximum value used when writing the value</param>
        /// <param name="numberOfBits">The number of bits used when writing the value</param>
        /// <returns>A floating point value larger or equal to MIN and smaller or equal to MAX</returns>
        public float ReadRangedSingle(float min, float max, int numberOfBits)
		{
			float range = max - min;
			int maxVal = (1 << numberOfBits) - 1;
			float encodedVal = (float)ReadUInt32(numberOfBits);
			float unit = encodedVal / (float)maxVal;
			return min + (unit * range);
		}

        public BitReader ReadRangedSingle(float min, float max, int numberOfBits, out float value)
        {
            value = ReadRangedSingle(min, max, numberOfBits);

            return this;
        }

        /// <summary>
        /// Reads a 32 bit integer value written using WriteRangedInteger()
        /// </summary>
        /// <param name="min">The minimum value used when writing the value</param>
        /// <param name="max">The maximum value used when writing the value</param>
        /// <returns>A signed integer value larger or equal to MIN and smaller or equal to MAX</returns>
        public int ReadRangedInteger(int min, int max)
		{
			uint range = (uint)(max - min);
			int numBits = BitUtility.BitsToHoldUInt(range);

			uint rvalue = ReadUInt32(numBits);
			return (int)(min + rvalue);
		}

        public BitReader ReadRangedInteger(int min, int max, out int value)
        {
            value = ReadRangedInteger(min, max);

            return this;
        }

        /// <summary>
        /// Reads a 64 bit integer value written using WriteRangedInteger() (64 version)
        /// </summary>
        /// <param name="min">The minimum value used when writing the value</param>
        /// <param name="max">The maximum value used when writing the value</param>
        /// <returns>A signed integer value larger or equal to MIN and smaller or equal to MAX</returns>
        public long ReadRangedInteger(long min, long max)
	    {
	        ulong range = (ulong)(max - min);
	        int numBits = BitUtility.BitsToHoldUInt64(range);
	
	        ulong rvalue = ReadUInt64(numBits);
	        return min + (long)rvalue;
	    }

        public BitReader ReadRangedInteger(long min, long max, out long value)
        {
            value = ReadRangedInteger(min, max);

            return this;
        }

        /// <summary>
        /// Reads a string written using Write(string)
        /// </summary>
        public string ReadString()
		{
			int byteLen = (int)ReadVariableUInt32();

			if (byteLen <= 0)
				return String.Empty;

			if ((ulong)(_lengthBits - _readPosition) < ((ulong)byteLen * 8))
			{
				// not enough data
				throw new BitBufferException(ReadOverflowError);
			}

			if ((_readPosition & 7) == 0)
			{
				// read directly
				string retval = System.Text.Encoding.UTF8.GetString(_data, _readPosition >> 3, byteLen);
				_readPosition += (8 * byteLen);
				return retval;
			}

			if (byteLen <= BufferSize) {
				byte[] buffer = (byte[]) Interlocked.Exchange(ref _buffer, null) ?? new byte[BufferSize];
				ReadBytes(buffer, 0, byteLen);
				string retval = Encoding.UTF8.GetString(buffer, 0, byteLen);
				_buffer = buffer;
				return retval;
			} else {
				byte[] bytes = ReadBytes(byteLen);
				return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			}
		}

        public BitReader ReadString(out string value)
        {
            value = ReadString();

            return this;
        }

        /// <summary>
        /// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
        /// </summary>
        public BitReader SkipPadBits()
		{
			_readPosition = ((_readPosition + 7) >> 3) * 8;

            return this;
		}

		/// <summary>
		/// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
		/// </summary>
		public BitReader ReadPadBits()
		{
			_readPosition = ((_readPosition + 7) >> 3) * 8;
            
            return this;
		}

		/// <summary>
		/// Pads data with the specified number of bits.
		/// </summary>
		public BitReader SkipPadBits(int numberOfBits)
		{
			_readPosition += numberOfBits;

            return this;
		}
	}
}
