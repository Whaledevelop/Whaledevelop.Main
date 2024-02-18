using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Whaledevelop.Serialization
{
    public class ByteWriter
    {
        private const int DEFAULT_CAPACITY = 1 << 10;
        private static readonly UTF8Encoding U_TF8_ENCODING = new(false, true);
        private byte[] _buffer;

        private int _position;

        public ByteWriter()
        {
            _buffer = new byte[DEFAULT_CAPACITY];
        }

        public ByteWriter(int capacity)
        {
            _buffer = new byte[capacity];
        }

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }

        public void Reset()
        {
            _position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> ToArraySegment()
        {
            return new(_buffer, 0, _position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureFit(int additionalSize)
        {
            if (_buffer.Length < _position + additionalSize)
            {
                Array.Resize(ref _buffer, Math.Max(_position + additionalSize, _buffer.Length * 2));
            }
        }

        public void WriteArraySegment(ArraySegment<byte> segment)
        {
            WriteInt(segment.Count);
            EnsureFit(segment.Count);
            // ReSharper disable once AssignNullToNotNullAttribute
            Array.Copy(segment.Array, segment.Offset, _buffer, _position, segment.Count);
            _position += segment.Count;
        }

        public void WriteByte(byte value)
        {
            EnsureFit(1);
            _buffer[_position++] = value;
        }

        public void WriteBytes(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "The input byte array is null.");
            }

            EnsureFit(value.Length);

            if (_position + value.Length > _buffer.Length)
            {
                throw new IndexOutOfRangeException("Attempted to write beyond the end of the buffer.");
            }

            WriteInt(value.Length);
            EnsureFit(value.Length);
            Array.Copy(value, 0, _buffer, _position, value.Length);
            _position += value.Length;
        }

        public void WriteBool(bool value)
        {
            EnsureFit(1);
            _buffer[_position++] = (byte)(value ? 1 : 0);
        }

        public void WriteUShort(ushort value)
        {
            EnsureFit(2);
            ByteBigConverter.FromUShort(value, _buffer, _position);
            _position += 2;
        }

        public void WriteShort(short value)
        {
            EnsureFit(2);
            ByteBigConverter.FromShort(value, _buffer, _position);
            _position += 2;
        }

        public void WriteUInt(uint value)
        {
            EnsureFit(4);
            ByteBigConverter.FromUInt(value, _buffer, _position);
            _position += 4;
        }

        public void WriteInt(int value)
        {
            EnsureFit(4);
            ByteBigConverter.FromInt(value, _buffer, _position);
            _position += 4;
        }

        public void WriteULong(ulong value)
        {
            EnsureFit(8);
            ByteBigConverter.FromULong(value, _buffer, _position);
            _position += 8;
        }

        public void WriteLong(long value)
        {
            EnsureFit(8);
            ByteBigConverter.FromLong(value, _buffer, _position);
            _position += 8;
        }

        public void WriteFloat(float value)
        {
            EnsureFit(4);
            ByteBigConverter.FromFloat(value, _buffer, _position);
            _position += 4;
        }

        public void WriteString(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "The input string is null.");
            }

            var byteCount = U_TF8_ENCODING.GetByteCount(value);
            WriteUShort((ushort)byteCount);
            EnsureFit(byteCount);
            var bytesWritten = U_TF8_ENCODING.GetBytes(value, 0, value.Length, _buffer, _position);
            _position += bytesWritten;
        }

        public void WriteSerializableData<T>(in T data)
            where T : ISerializableData
        {
            data.ToByteWriter(this);
        }
    }
}