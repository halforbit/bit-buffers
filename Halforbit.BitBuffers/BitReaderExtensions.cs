using System;
using System.Collections.Generic;
using System.Numerics;

namespace Halforbit.BitBuffers
{
    public static class BitReaderExtensions
    {
        static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static BitReader WithEach<TItem>(
            this BitReader bitReader,
            IEnumerable<TItem> enumerable,
            Action<TItem> action)
        {
            foreach (var item in enumerable) action(item);

            return bitReader;
        }

        public static BitReader WithEach<TItem>(
            this BitReader bitReader,
            IEnumerable<TItem> enumerable,
            Action<BitReader, TItem> action)
        {
            foreach (var item in enumerable) action(bitReader, item);

            return bitReader;
        }

        public static BitReader ReadList<TElement>(
            this BitReader bitReader,
            Func<BitReader, TElement> func,
            out IReadOnlyList<TElement> value)
        {
            var count = bitReader.ReadVariableUInt32();

            var list = new List<TElement>((int)count);

            for (var i = 0; i < count; i++)
            {
                list[i] = func(bitReader);
            }

            value = list;

            return bitReader;
        }

        public static IReadOnlyList<TElement> ReadList<TElement>(
            this BitReader bitReader,
            Func<BitReader, TElement> func)
        {
            var count = bitReader.ReadVariableUInt32();

            var list = new List<TElement>((int)count);

            for (var i = 0; i < count; i++)
            {
                list[i] = func(bitReader);
            }

            return list;
        }

        public static DateTime ReadEpochSeconds(
            this BitReader bitReader)
        {
            return _epoch.AddSeconds(bitReader.ReadVariableUInt64());
        }

        public static BitReader ReadEpochSeconds(
            this BitReader bitReader,
            out DateTime time)
        {
            time = _epoch.AddSeconds(bitReader.ReadVariableUInt64());

            return bitReader;
        }

        public static BigInteger ReadBigInteger(
            this BitReader bitReader)
        {
            var length = bitReader.ReadVariableUInt32();

            var bytes = bitReader.ReadBytes((int)length);

            return new BigInteger(bytes);
        }

        public static BitReader ReadBigInteger(
            this BitReader bitReader,
            out BigInteger value)
        {
            value = bitReader.ReadBigInteger();

            return bitReader;
        }

        public static TEnum ReadEnum<TEnum>(
            this BitReader bitReader)
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enum type.");
            }

            var typeCode = default(TEnum).GetTypeCode();

            switch (typeCode)
            {
                case TypeCode.Byte: return (TEnum)(object)bitReader.ReadByte();

                case TypeCode.SByte: return (TEnum)(object)bitReader.ReadSByte();

                case TypeCode.Int16: return (TEnum)(object)bitReader.ReadInt16();

                case TypeCode.UInt16: return (TEnum)(object)bitReader.ReadUInt16();

                case TypeCode.Int32: return (TEnum)(object)bitReader.ReadVariableInt32();

                case TypeCode.UInt32: return (TEnum)(object)bitReader.ReadVariableUInt32();

                case TypeCode.Int64: return (TEnum)(object)bitReader.ReadVariableInt64();

                case TypeCode.UInt64: return (TEnum)(object)bitReader.ReadVariableUInt64();

                default: throw new ArgumentException($"Enum {type.Name} is of unsupported type `{typeCode}`.");
            }
        }

        public static BitReader ReadEnum<TEnum>(
            this BitReader bitReader,
            out TEnum value) 
            where TEnum : struct, IConvertible
        {
            value = bitReader.ReadEnum<TEnum>();

            return bitReader;
        }

        public static TEnum ReadEnum<TEnum>(
            this BitReader bitReader,
            int numberOfBits) 
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enum type.");
            }

            var typeCode = default(TEnum).GetTypeCode();

            switch (typeCode)
            {
                case TypeCode.Byte: return (TEnum)(object)bitReader.ReadByte(numberOfBits);

                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    throw new NotSupportedException(
                        $"Enum {type.Name} is of type `{typeCode}` which does not support a specified number of bits.");

                case TypeCode.Int32: return (TEnum)(object)bitReader.ReadInt32(numberOfBits);

                case TypeCode.UInt32: return (TEnum)(object)bitReader.ReadUInt32(numberOfBits);

                case TypeCode.Int64: return (TEnum)(object)bitReader.ReadInt64(numberOfBits);

                case TypeCode.UInt64: return (TEnum)(object)bitReader.ReadUInt64(numberOfBits);

                default: throw new ArgumentException($"Enum {type.Name} is of unsupported type `{typeCode}`.");
            }
        }

        public static BitReader ReadEnum<TEnum>(
            this BitReader bitReader,
            int numberOfBits, 
            out TEnum value) 
            where TEnum : struct, IConvertible
        {
            value = bitReader.ReadEnum<TEnum>(numberOfBits);

            return bitReader;
        }
    }
}
