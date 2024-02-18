using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Whaledevelop.Serialization
{
    public static class ByteBigConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromShort(short value, byte[] array, int startIndex)
        {
            array[startIndex + 1] = (byte)value;
            array[startIndex] = (byte)(value >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ToShort(byte[] array, int startIndex)
        {
            return (short)(array[startIndex + 1] | (array[startIndex] << 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromUShort(ushort value, byte[] array, int startIndex)
        {
            array[startIndex + 1] = (byte)value;
            array[startIndex] = (byte)(value >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUShort(byte[] array, int startIndex)
        {
            return (ushort)(array[startIndex + 1] | (array[startIndex] << 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromInt(int value, byte[] array, int startIndex)
        {
            array[startIndex + 3] = (byte)value;
            array[startIndex + 2] = (byte)(value >> 8);
            array[startIndex + 1] = (byte)(value >> 16);
            array[startIndex] = (byte)(value >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(byte[] array, int startIndex)
        {
            return array[startIndex + 3] | (array[startIndex + 2] << 8) | (array[startIndex + 1] << 16) | (array[startIndex] << 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromUInt(uint value, byte[] array, int startIndex)
        {
            array[startIndex + 3] = (byte)value;
            array[startIndex + 2] = (byte)(value >> 8);
            array[startIndex + 1] = (byte)(value >> 16);
            array[startIndex] = (byte)(value >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt(byte[] array, int startIndex)
        {
            return (uint)(array[startIndex + 3] | (array[startIndex + 2] << 8) | (array[startIndex + 1] << 16) | (array[startIndex] << 24));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromLong(long value, byte[] array, int startIndex)
        {
            array[startIndex + 7] = (byte)value;
            array[startIndex + 6] = (byte)(value >> 8);
            array[startIndex + 5] = (byte)(value >> 16);
            array[startIndex + 4] = (byte)(value >> 24);
            array[startIndex + 3] = (byte)(value >> 32);
            array[startIndex + 2] = (byte)(value >> 40);
            array[startIndex + 1] = (byte)(value >> 48);
            array[startIndex] = (byte)(value >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToLong(byte[] array, int startIndex)
        {
            return array[startIndex + 7] | ((long)array[startIndex + 6] << 8)
                | ((long)array[startIndex + 5] << 16) | ((long)array[startIndex + 4] << 24) | ((long)array[startIndex + 3] << 32)
                | ((long)array[startIndex + 2] << 40) | ((long)array[startIndex + 1] << 48) | ((long)array[startIndex] << 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromULong(ulong value, byte[] array, int startIndex)
        {
            array[startIndex + 7] = (byte)value;
            array[startIndex + 6] = (byte)(value >> 8);
            array[startIndex + 5] = (byte)(value >> 16);
            array[startIndex + 4] = (byte)(value >> 24);
            array[startIndex + 3] = (byte)(value >> 32);
            array[startIndex + 2] = (byte)(value >> 40);
            array[startIndex + 1] = (byte)(value >> 48);
            array[startIndex] = (byte)(value >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToULong(byte[] array, int startIndex)
        {
            return array[startIndex + 7] | ((ulong)array[startIndex + 6] << 8)
                | ((ulong)array[startIndex + 5] << 16) | ((ulong)array[startIndex + 4] << 24) | ((ulong)array[startIndex + 3] << 32)
                | ((ulong)array[startIndex + 2] << 40) | ((ulong)array[startIndex + 1] << 48) | ((ulong)array[startIndex] << 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromFloat(float value, byte[] array, int startIndex)
        {
            var converter = new FloatConverter
            {
                Value = value
            };
            array[startIndex + 3] = converter.Byte0;
            array[startIndex + 2] = converter.Byte1;
            array[startIndex + 1] = converter.Byte2;
            array[startIndex] = converter.Byte3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(byte[] array, int startIndex)
        {
            return new FloatConverter
            {
                Byte0 = array[startIndex + 3],
                Byte1 = array[startIndex + 2],
                Byte2 = array[startIndex + 1],
                Byte3 = array[startIndex]
            }.Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct FloatConverter
        {
            [FieldOffset(0)]
            public byte Byte0;
            [FieldOffset(1)]
            public byte Byte1;
            [FieldOffset(2)]
            public byte Byte2;
            [FieldOffset(3)]
            public byte Byte3;
            [FieldOffset(0)]
            public float Value;
        }
    }
}