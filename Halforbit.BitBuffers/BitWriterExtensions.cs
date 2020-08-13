using System;
using System.Collections.Generic;
using System.Numerics;

namespace Halforbit.BitBuffers
{
    public static class BitWriterExtensions
    {
        static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static BitWriter WithEach<TItem>(
            this BitWriter bitWriter,
            IEnumerable<TItem> enumerable,
            Action<TItem> action)
        {
            foreach (var item in enumerable) action(item);

            return bitWriter;
        }

        public static BitWriter WithEach<TItem>(
            this BitWriter bitWriter,
            IEnumerable<TItem> enumerable,
            Action<BitWriter, TItem> action)
        {
            foreach (var item in enumerable) action(bitWriter, item);

            return bitWriter;
        }

        public static BitWriter WriteList<TItem>(
            this BitWriter bitWriter,
            IReadOnlyList<TItem> list,
            Action<BitWriter, TItem> writeItem)
        {
            bitWriter.WriteVariableUInt32((uint)list.Count);

            foreach (var item in list)
            {
                writeItem(bitWriter, item);
            }

            return bitWriter;
        }

        public static BitWriter WriteEpochSeconds(
            this BitWriter bitWriter,
            DateTime time)
        {
            return bitWriter.WriteVariableUInt64((ulong)(time - _epoch).TotalSeconds);
        }

        public static BitWriter WriteEpochMilliseconds(
            this BitWriter bitWriter,
            DateTime time)
        {
            return bitWriter.WriteVariableUInt64((ulong)(time - _epoch).TotalMilliseconds);
        }

        public static BitWriter Write(
            this BitWriter bitWriter,
            Guid guid)
        {
            return bitWriter.Write(guid.ToByteArray());
        }

        public static BitWriter Write(
            this BitWriter bitWriter,
            BigInteger bigInteger)
        {
            var bytes = bigInteger.ToByteArray();

            var length = (uint)bytes.Length;

            return bitWriter
                .WriteVariableUInt32(length)
                .Write(bytes, 0, (int)length);
        }

        public static BitWriter WriteEnum<TEnum>(
            this BitWriter bitWriter,
            TEnum enumValue) 
            where TEnum : struct, IConvertible
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
                case byte b: bitWriter.Write(b); break;

                case sbyte sb: bitWriter.Write(sb); break;

                case short i16: bitWriter.Write(i16); break;

                case ushort u16: bitWriter.Write(u16); break;

                case int i32: bitWriter.WriteVariableInt32(i32); break;

                case uint u32: bitWriter.WriteVariableUInt32(u32); break;

                case long i64: bitWriter.WriteVariableInt64(i64); break;

                case ulong u64: bitWriter.WriteVariableUInt64(u64); break;

                default: throw new ArgumentException($"Enum {type.Name} is of unsupported type `{typeCode}`.");
            }

            return bitWriter;
        }

        public static BitWriter WriteEnum<TEnum>(
            this BitWriter bitWriter, 
            TEnum enumValue, 
            int numberOfBits) 
            where TEnum : struct, IConvertible
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
                case byte b: bitWriter.Write(b, numberOfBits); break;

                case sbyte sb:
                case short i16:
                case ushort u16:
                    throw new NotSupportedException(
                        $"Enum {type.Name} is of type `{typeCode}` which does not support a specified number of bits.");

                case int i32: bitWriter.Write(i32, numberOfBits); break;

                case uint u32: bitWriter.Write(u32, numberOfBits); break;

                case long i64: bitWriter.Write(i64, numberOfBits); break;

                case ulong u64: bitWriter.Write(u64, numberOfBits); break;

                default: throw new ArgumentException($"Enum {type.Name} is of unsupported type `{typeCode}`.");
            }

            return bitWriter;
        }
    }
}
