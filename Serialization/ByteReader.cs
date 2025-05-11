using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace bog.Serialization
{
    public class ByteReader
    {
        private static readonly UTF8Encoding UTF8_ENCODING = new(false, true);

        private byte[] _buffer = Array.Empty<byte>();
        private int _length;
        private int _position;
        private int _positionLength;

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _position;
        }

        public int BytesAvailable
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _positionLength - _position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSource(ArraySegment<byte> segment)
        {
            _buffer = segment.Array;
            _length = segment.Count;
            _position = segment.Offset;
            _positionLength = _length + _position;
        }

        public ArraySegment<byte> ReadArraySegment()
        {
            var length = ReadInt();
            return ReadArraySegment(length);
        }

        public ArraySegment<byte> ReadArraySegment(int count)
        {
            Debug.Assert(_position + count <= _positionLength);
            var segment = new ArraySegment<byte>(_buffer, _position, count);
            _position += count;
            return segment;
        }

        public byte ReadByte()
        {
            Debug.Assert(_position + 1 <= _positionLength);
            var res = _buffer[_position];
            _position += 1;
            return res;
        }

        public bool ReadBool()
        {
            Debug.Assert(_position + 1 <= _positionLength);
            return ReadByte() > 0;
        }

        public int ReadInt()
        {
            Debug.Assert(_position + 4 <= _positionLength);
            var result = ByteBigConverter.ToInt(_buffer, _position);
            _position += 4;
            return result;
        }

        public uint ReadUInt()
        {
            Debug.Assert(_position + 4 <= _positionLength);
            var result = ByteBigConverter.ToUInt(_buffer, _position);
            _position += 4;
            return result;
        }

        public short ReadShort()
        {
            Debug.Assert(_position + 2 <= _positionLength);
            var result = ByteBigConverter.ToShort(_buffer, _position);
            _position += 2;
            return result;
        }

        public ushort ReadUShort()
        {
            Debug.Assert(_position + 2 <= _positionLength);
            var result = ByteBigConverter.ToUShort(_buffer, _position);
            _position += 2;
            return result;
        }

        public long ReadLong()
        {
            Debug.Assert(_position + 8 <= _positionLength);
            var result = ByteBigConverter.ToLong(_buffer, _position);
            _position += 8;
            return result;
        }

        public ulong ReadULong()
        {
            Debug.Assert(_position + 8 <= _positionLength);
            var result = ByteBigConverter.ToULong(_buffer, _position);
            _position += 8;
            return result;
        }

        public float ReadFloat()
        {
            Debug.Assert(_position + 4 <= _positionLength);
            var result = ByteBigConverter.ToFloat(_buffer, _position);
            _position += 4;
            return result;
        }

        public string ReadString()
        {
            int length = ReadUShort();
            if (length == 0)
            {
                return string.Empty;
            }

            Debug.Assert(_position + length <= _positionLength);

            var result = UTF8_ENCODING.GetString(_buffer, _position, length);
            _position += length;
            return result;
        }

        public void ReadSerializableData<T>(ref T serializableData)
            where T : ISerializableData
        {
            serializableData.FromByteReader(this);
        }
    }
}