using System;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Lidgren.Network
{
    /// <summary>
    /// Utility struct for writing Singles
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct SingleUIntUnion
    {
        /// <summary>
        /// Value as a 32 bit float
        /// </summary>
        [FieldOffset(0)]
        public float SingleValue;

        /// <summary>
        /// Value as an unsigned 32 bit integer
        /// </summary>
        [FieldOffset(0)]
        public uint UIntValue;
    }

    public partial class BitBuffer
    {
        /// <summary>
        /// Number of bytes to overallocate for each message to avoid resizing
        /// </summary>
        const int OverAllocateAmount = 4;

        /// <summary>
        /// Ensures the buffer can hold this number of bits
        /// </summary>
        void EnsureBufferSize(int numberOfBits)
        {
            int byteLen = ((numberOfBits + 7) >> 3);
            if (_data == null)
            {
                _data = new byte[byteLen + OverAllocateAmount];
                return;
            }
            if (_data.Length < byteLen)
                Array.Resize<byte>(ref _data, byteLen + OverAllocateAmount);
            return;
        }

        /// <summary>
        /// Writes a boolean value using 1 bit
        /// </summary>
        public BitBuffer Write(bool value)
        {
            EnsureBufferSize(_lengthBits + 1);
            BitReaderWriter.WriteByte((value ? (byte)1 : (byte)0), 1, _data, _lengthBits);
            _lengthBits += 1;

            return this;
        }

        /// <summary>
        /// Write a byte
        /// </summary>
        public BitBuffer Write(byte source)
        {
            EnsureBufferSize(_lengthBits + 8);
            BitReaderWriter.WriteByte(source, 8, _data, _lengthBits);
            _lengthBits += 8;

            return this;
        }

        /// <summary>
        /// Writes a byte at a given offset in the buffer
        /// </summary>
        public BitBuffer WriteAt(Int32 offset, byte source)
        {
            int newBitLength = Math.Max(_lengthBits, offset + 8);
            EnsureBufferSize(newBitLength);
            BitReaderWriter.WriteByte((byte)source, 8, _data, offset);
            _lengthBits = newBitLength;

            return this;
        }

        /// <summary>
        /// Writes a signed byte
        /// </summary>
        public BitBuffer Write(sbyte source)
        {
            EnsureBufferSize(_lengthBits + 8);
            BitReaderWriter.WriteByte((byte)source, 8, _data, _lengthBits);
            _lengthBits += 8;

            return this;
        }

        /// <summary>
        /// Writes 1 to 8 bits of a byte
        /// </summary>
        public BitBuffer Write(byte source, int numberOfBits)
        {
            BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 8), "Write(byte, numberOfBits) can only write between 1 and 8 bits");
            EnsureBufferSize(_lengthBits + numberOfBits);
            BitReaderWriter.WriteByte(source, numberOfBits, _data, _lengthBits);
            _lengthBits += numberOfBits;

            return this;
        }

        /// <summary>
        /// Writes all bytes in an array
        /// </summary>
        public BitBuffer Write(byte[] source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            int bits = source.Length * 8;
            EnsureBufferSize(_lengthBits + bits);
            BitReaderWriter.WriteBytes(source, 0, source.Length, _data, _lengthBits);
            _lengthBits += bits;

            return this;
        }

        /// <summary>
        /// Writes the specified number of bytes from an array
        /// </summary>
        public BitBuffer Write(byte[] source, int offsetInBytes, int numberOfBytes)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            int bits = numberOfBytes * 8;
            EnsureBufferSize(_lengthBits + bits);
            BitReaderWriter.WriteBytes(source, offsetInBytes, numberOfBytes, _data, _lengthBits);
            _lengthBits += bits;

            return this;
        }

        /// <summary>
        /// Writes an unsigned 16 bit integer
        /// </summary>
        /// <param name="source"></param>
        public BitBuffer Write(UInt16 source)
        {
            EnsureBufferSize(_lengthBits + 16);
            BitReaderWriter.WriteUInt16(source, 16, _data, _lengthBits);
            _lengthBits += 16;

            return this;
        }

        /// <summary>
        /// Writes a 16 bit unsigned integer at a given offset in the buffer
        /// </summary>
        public BitBuffer WriteAt(Int32 offset, UInt16 source)
        {
            int newBitLength = Math.Max(_lengthBits, offset + 16);
            EnsureBufferSize(newBitLength);
            BitReaderWriter.WriteUInt16(source, 16, _data, offset);
            _lengthBits = newBitLength;

            return this;
        }

        /// <summary>
        /// Writes an unsigned integer using 1 to 16 bits
        /// </summary>
        public BitBuffer Write(UInt16 source, int numberOfBits)
        {
            BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 16), "Write(ushort, numberOfBits) can only write between 1 and 16 bits");
            EnsureBufferSize(_lengthBits + numberOfBits);
            BitReaderWriter.WriteUInt16(source, numberOfBits, _data, _lengthBits);
            _lengthBits += numberOfBits;

            return this;
        }

        /// <summary>
        /// Writes a signed 16 bit integer
        /// </summary>
        public BitBuffer Write(Int16 source)
        {
            EnsureBufferSize(_lengthBits + 16);
            BitReaderWriter.WriteUInt16((ushort)source, 16, _data, _lengthBits);
            _lengthBits += 16;

            return this;
        }

        /// <summary>
        /// Writes a 16 bit signed integer at a given offset in the buffer
        /// </summary>
        public BitBuffer WriteAt(Int32 offset, Int16 source)
        {
            int newBitLength = Math.Max(_lengthBits, offset + 16);
            EnsureBufferSize(newBitLength);
            BitReaderWriter.WriteUInt16((ushort)source, 16, _data, offset);
            _lengthBits = newBitLength;

            return this;
        }

#if UNSAFE
		/// <summary>
		/// Writes a 32 bit signed integer
		/// </summary>
		public unsafe BitBuffer Write(Int32 source)
		{
			EnsureBufferSize(m_bitLength + 32);

			// can write fast?
			if (m_bitLength % 8 == 0)
			{
				fixed (byte* numRef = &Data[m_bitLength / 8])
				{
					*((int*)numRef) = source;
				}
			}
			else
			{
				NetBitWriter.WriteUInt32((UInt32)source, 32, Data, m_bitLength);
			}
			m_bitLength += 32;

            return this;
		}
#else
        /// <summary>
        /// Writes a 32 bit signed integer
        /// </summary>
        public BitBuffer Write(Int32 source)
        {
            EnsureBufferSize(_lengthBits + 32);
            BitReaderWriter.WriteUInt32((UInt32)source, 32, _data, _lengthBits);
            _lengthBits += 32;

            return this;
        }
#endif

        /// <summary>
        /// Writes a 32 bit signed integer at a given offset in the buffer
        /// </summary>
        public BitBuffer WriteAt(Int32 offset, Int32 source)
        {
            int newBitLength = Math.Max(_lengthBits, offset + 32);
            EnsureBufferSize(newBitLength);
            BitReaderWriter.WriteUInt32((UInt32)source, 32, _data, offset);
            _lengthBits = newBitLength;

            return this;
        }

#if UNSAFE
		/// <summary>
		/// Writes a 32 bit unsigned integer
		/// </summary>
		public unsafe BitBuffer Write(UInt32 source)
		{
			EnsureBufferSize(m_bitLength + 32);

			// can write fast?
			if (m_bitLength % 8 == 0)
			{
				fixed (byte* numRef = &Data[m_bitLength / 8])
				{
					*((uint*)numRef) = source;
				}
			}
			else
			{
				NetBitWriter.WriteUInt32(source, 32, Data, m_bitLength);
			}

			m_bitLength += 32;

            return this;
		}
#else
        /// <summary>
        /// Writes a 32 bit unsigned integer
        /// </summary>
        public BitBuffer Write(UInt32 source)
        {
            EnsureBufferSize(_lengthBits + 32);
            BitReaderWriter.WriteUInt32(source, 32, _data, _lengthBits);
            _lengthBits += 32;

            return this;
        }
#endif

        /// <summary>
        /// Writes a 32 bit unsigned integer at a given offset in the buffer
        /// </summary>
        public BitBuffer WriteAt(Int32 offset, UInt32 source)
        {
            int newBitLength = Math.Max(_lengthBits, offset + 32);
            EnsureBufferSize(newBitLength);
            BitReaderWriter.WriteUInt32(source, 32, _data, offset);
            _lengthBits = newBitLength;

            return this;
        }

        /// <summary>
        /// Writes a 32 bit signed integer
        /// </summary>
        public BitBuffer Write(UInt32 source, int numberOfBits)
        {
            BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 32), "Write(uint, numberOfBits) can only write between 1 and 32 bits");
            EnsureBufferSize(_lengthBits + numberOfBits);
            BitReaderWriter.WriteUInt32(source, numberOfBits, _data, _lengthBits);
            _lengthBits += numberOfBits;

            return this;
        }

        /// <summary>
        /// Writes a signed integer using 1 to 32 bits
        /// </summary>
        public BitBuffer Write(Int32 source, int numberOfBits)
        {
            BitBufferException.Assert((numberOfBits > 0 && numberOfBits <= 32), "Write(int, numberOfBits) can only write between 1 and 32 bits");
            EnsureBufferSize(_lengthBits + numberOfBits);

            if (numberOfBits != 32)
            {
                // make first bit sign
                int signBit = 1 << (numberOfBits - 1);
                if (source < 0)
                    source = (-source - 1) | signBit;
                else
                    source &= (~signBit);
            }

            BitReaderWriter.WriteUInt32((uint)source, numberOfBits, _data, _lengthBits);

            _lengthBits += numberOfBits;

            return this;
        }

        /// <summary>
        /// Writes a 64 bit unsigned integer
        /// </summary>
        public BitBuffer Write(UInt64 source)
        {
            EnsureBufferSize(_lengthBits + 64);
            BitReaderWriter.WriteUInt64(source, 64, _data, _lengthBits);
            _lengthBits += 64;

            return this;
        }

        /// <summary>
        /// Writes a 64 bit unsigned integer at a given offset in the buffer
        /// </summary>
        public BitBuffer WriteAt(Int32 offset, UInt64 source)
        {
            int newBitLength = Math.Max(_lengthBits, offset + 64);
            EnsureBufferSize(newBitLength);
            BitReaderWriter.WriteUInt64(source, 64, _data, offset);
            _lengthBits = newBitLength;

            return this;
        }

        /// <summary>
        /// Writes an unsigned integer using 1 to 64 bits
        /// </summary>
        public BitBuffer Write(UInt64 source, int numberOfBits)
        {
            EnsureBufferSize(_lengthBits + numberOfBits);
            BitReaderWriter.WriteUInt64(source, numberOfBits, _data, _lengthBits);
            _lengthBits += numberOfBits;

            return this;
        }

        /// <summary>
        /// Writes a 64 bit signed integer
        /// </summary>
        public BitBuffer Write(Int64 source)
        {
            EnsureBufferSize(_lengthBits + 64);
            ulong usource = (ulong)source;
            BitReaderWriter.WriteUInt64(usource, 64, _data, _lengthBits);
            _lengthBits += 64;

            return this;
        }

        /// <summary>
        /// Writes a signed integer using 1 to 64 bits
        /// </summary>
        public BitBuffer Write(Int64 source, int numberOfBits)
        {
            EnsureBufferSize(_lengthBits + numberOfBits);
            ulong usource = (ulong)source;
            BitReaderWriter.WriteUInt64(usource, numberOfBits, _data, _lengthBits);
            _lengthBits += numberOfBits;

            return this;
        }

        //
        // Floating point
        //
#if UNSAFE
		/// <summary>
		/// Writes a 32 bit floating point value
		/// </summary>
		public unsafe BitBuffer Write(float source)
		{
			uint val = *((uint*)&source);
#if BIGENDIAN
				val = NetUtility.SwapByteOrder(val);
#endif
			Write(val);

            return this;
		}
#else
        /// <summary>
        /// Writes a 32 bit floating point value
        /// </summary>
        public BitBuffer Write(float source)
        {
            // Use union to avoid BitConverter.GetBytes() which allocates memory on the heap
            SingleUIntUnion su;
            su.UIntValue = 0; // must initialize every member of the union to avoid warning
            su.SingleValue = source;

#if BIGENDIAN
			// swap byte order
			su.UIntValue = NetUtility.SwapByteOrder(su.UIntValue);
#endif
            Write(su.UIntValue);

            return this;
        }
#endif

#if UNSAFE
		/// <summary>
		/// Writes a 64 bit floating point value
		/// </summary>
		public unsafe BitBuffer Write(double source)
		{
			ulong val = *((ulong*)&source);
#if BIGENDIAN
			val = NetUtility.SwapByteOrder(val);
#endif
			Write(val);

            return this;
		}
#else
        /// <summary>
        /// Writes a 64 bit floating point value
        /// </summary>
        public BitBuffer Write(double source)
        {
            byte[] val = BitConverter.GetBytes(source);
#if BIGENDIAN
			// 0 1 2 3   4 5 6 7

			// swap byte order
			byte tmp = val[7];
			val[7] = val[0];
			val[0] = tmp;

			tmp = val[6];
			val[6] = val[1];
			val[1] = tmp;

			tmp = val[5];
			val[5] = val[2];
			val[2] = tmp;

			tmp = val[4];
			val[4] = val[3];
			val[3] = tmp;
#endif
            Write(val);

            return this;
        }
#endif

        //
        // Variable bits
        //

        /// <summary>
        /// Write Base128 encoded variable sized unsigned integer of up to 32 bits
        /// </summary>
        /// <returns>number of bytes written</returns>
        public BitBuffer WriteVariableUInt32(uint value, out int bytesWritten)
        {
            int retval = 1;
            uint num1 = (uint)value;
            while (num1 >= 0x80)
            {
                this.Write((byte)(num1 | 0x80));
                num1 = num1 >> 7;
                retval++;
            }
            this.Write((byte)num1);

            bytesWritten = retval;

            return this;
        }

        public BitBuffer WriteVariableUInt32(uint value) => WriteVariableUInt32(value, out var _);

        /// <summary>
        /// Write Base128 encoded variable sized signed integer of up to 32 bits
        /// </summary>
        /// <returns>number of bytes written</returns>
        public BitBuffer WriteVariableInt32(int value, out int bytesWritten)
        {
            uint zigzag = (uint)(value << 1) ^ (uint)(value >> 31);
            
            WriteVariableUInt32(zigzag, out bytesWritten);

            return this;
        }

        public BitBuffer WriteVariableInt32(int value) => WriteVariableInt32(value, out var _);

        /// <summary>
        /// Write Base128 encoded variable sized signed integer of up to 64 bits
        /// </summary>
        /// <returns>number of bytes written</returns>
        public BitBuffer WriteVariableInt64(Int64 value, out int bytesWritten)
        {
            ulong zigzag = (ulong)(value << 1) ^ (ulong)(value >> 63);
            
            WriteVariableUInt64(zigzag, out bytesWritten);

            return this;
        }

        public BitBuffer WriteVariableInt64(Int64 value) => WriteVariableInt64(value, out var _);

        /// <summary>
        /// Write Base128 encoded variable sized unsigned integer of up to 64 bits
        /// </summary>
        /// <returns>number of bytes written</returns>
        public BitBuffer WriteVariableUInt64(UInt64 value, out int bytesWritten)
        {
            int retval = 1;
            UInt64 num1 = (UInt64)value;
            while (num1 >= 0x80)
            {
                this.Write((byte)(num1 | 0x80));
                num1 = num1 >> 7;
                retval++;
            }
            this.Write((byte)num1);
            
            bytesWritten = retval;

            return this;
        }

        public BitBuffer WriteVariableUInt64(UInt64 value) => WriteVariableUInt64(value, out var _);

        /// <summary>
        /// Compress (lossy) a float in the range -1..1 using numberOfBits bits
        /// </summary>
        public BitBuffer WriteSignedSingle(float value, int numberOfBits)
        {
            BitBufferException.Assert(((value >= -1.0) && (value <= 1.0)), " WriteSignedSingle() must be passed a float in the range -1 to 1; val is " + value);

            float unit = (value + 1.0f) * 0.5f;
            int maxVal = (1 << numberOfBits) - 1;
            uint writeVal = (uint)(unit * (float)maxVal);

            Write(writeVal, numberOfBits);

            return this;
        }

        /// <summary>
        /// Compress (lossy) a float in the range 0..1 using numberOfBits bits
        /// </summary>
        public BitBuffer WriteUnitSingle(float value, int numberOfBits)
        {
            BitBufferException.Assert(((value >= 0.0) && (value <= 1.0)), " WriteUnitSingle() must be passed a float in the range 0 to 1; val is " + value);

            int maxValue = (1 << numberOfBits) - 1;
            uint writeVal = (uint)(value * (float)maxValue);

            Write(writeVal, numberOfBits);

            return this;
        }

        /// <summary>
        /// Compress a float within a specified range using a certain number of bits
        /// </summary>
        public BitBuffer WriteRangedSingle(float value, float min, float max, int numberOfBits)
        {
            BitBufferException.Assert(((value >= min) && (value <= max)), " WriteRangedSingle() must be passed a float in the range MIN to MAX; val is " + value);

            float range = max - min;
            float unit = ((value - min) / range);
            int maxVal = (1 << numberOfBits) - 1;
            Write((UInt32)((float)maxVal * unit), numberOfBits);

            return this;
        }

        /// <summary>
        /// Writes an integer with the least amount of bits need for the specified range
        /// Returns number of bits written
        /// </summary>
        public BitBuffer WriteRangedInteger(int min, int max, int value, out int bitsWritten)
        {
            BitBufferException.Assert(value >= min && value <= max, "Value not within min/max range!");

            uint range = (uint)(max - min);
            int numBits = BitUtility.BitsToHoldUInt(range);

            uint rvalue = (uint)(value - min);
            Write(rvalue, numBits);

            bitsWritten = numBits;
            
            return this;
        }

        public BitBuffer WriteRangedInteger(int min, int max, int value) => WriteRangedInteger(min, max, value, out var _);

        /// <summary>
        /// Writes an integer with the least amount of bits need for the specified range
        /// Returns number of bits written
        /// </summary>
        public BitBuffer WriteRangedInteger(long min, long max, long value, out int bitsWritten)
        {
            BitBufferException.Assert(value >= min && value <= max, "Value not within min/max range!");

            ulong range = (ulong)(max - min);
            int numBits = BitUtility.BitsToHoldUInt64(range);

            ulong rvalue = (ulong)(value - min);
            Write(rvalue, numBits);

            bitsWritten = numBits;

            return this;
        }

        public BitBuffer WriteRangedInteger(long min, long max, long value) => WriteRangedInteger(min, max, value, out var _);

        /// <summary>
        /// Write a string
        /// </summary>
        public BitBuffer Write(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                WriteVariableUInt32(0);
                
                return this;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(source);
            EnsureBufferSize(_lengthBits + 8 + (bytes.Length * 8));
            WriteVariableUInt32((uint)bytes.Length);
            Write(bytes);

            return this;
        }

        /// <summary>
        /// Writes a BigInteger
        /// </summary>
        public BitBuffer Write(BigInteger bigInteger)
        {
            var bytes = bigInteger.ToByteArray();
            
            WriteVariableUInt32((uint)bytes.Length);

            Write(bytes);

            return this;
        }

        public BitBuffer WriteEnum<TEnum>(TEnum enumValue) where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enum type.");
            }

            var typeCode = enumValue.GetTypeCode();

            var value = Convert.ChangeType(enumValue, typeCode);

            switch (value)
            {
                case byte b: Write(b); break;

                case sbyte sb: Write(sb); break;

                case short i16: Write(i16); break;

                case ushort u16: Write(u16); break;

                case int i32: WriteVariableInt32(i32); break;

                case uint u32: WriteVariableUInt32(u32); break;

                case long i64: WriteVariableInt64(i64); break;

                case ulong u64: WriteVariableUInt64(u64); break;

                default: throw new ArgumentException($"Enum {type.Name} is of unsupported type `{typeCode}`.");
            }

            return this;
        }

        public BitBuffer WriteEnum<TEnum>(TEnum enumValue, int numberOfBits) where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enum type.");
            }

            var typeCode = enumValue.GetTypeCode();

            var value = Convert.ChangeType(enumValue, typeCode);

            switch (value)
            {
                case byte b: Write(b, numberOfBits); break;

                case sbyte sb: 
                case short i16: 
                case ushort u16:
                    throw new NotSupportedException(
                        $"Enum {type.Name} is of type `{typeCode}` which does not support a specified number of bits.");

                case int i32: Write(i32, numberOfBits); break;

                case uint u32: Write(u32, numberOfBits); break;

                case long i64: Write(i64, numberOfBits); break;

                case ulong u64: Write(u64, numberOfBits); break;

                default: throw new ArgumentException($"Enum {type.Name} is of unsupported type `{typeCode}`.");
            }

            return this;
        }

        /// <summary>
        /// Writes a local timestamp to a message; readable (and convertable to local time) by the remote host using ReadTime()
        /// </summary>
        public BitBuffer WriteTime(double localTime, bool highPrecision)
        {
            if (highPrecision)
                Write(localTime);
            else
                Write((float)localTime);

            return this;
        }

        /// <summary>
        /// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
        /// </summary>
        public BitBuffer WritePadBits()
        {
            _lengthBits = ((_lengthBits + 7) >> 3) * 8;
            EnsureBufferSize(_lengthBits);

            return this;
        }

        /// <summary>
        /// Pads data with the specified number of bits.
        /// </summary>
        public BitBuffer WritePadBits(int numberOfBits)
        {
            _lengthBits += numberOfBits;
            EnsureBufferSize(_lengthBits);

            return this;
        }

        /// <summary>
        /// Append all the bits of message to this message
        /// </summary>
        public BitBuffer Write(BitBuffer buffer)
        {
            EnsureBufferSize(_lengthBits + (buffer.LengthBytes * 8));

            Write(buffer._data, 0, buffer.LengthBytes);

            // did we write excessive bits?
            int bitsInLastByte = (buffer._lengthBits % 8);
            if (bitsInLastByte != 0)
            {
                int excessBits = 8 - bitsInLastByte;
                _lengthBits -= excessBits;
            }

            return this;
        }
    }
}
