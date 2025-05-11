namespace bog.Serialization
{
    public interface ISerializableData
    {
        void ToByteWriter(ByteWriter byteWriter);

        void FromByteReader(ByteReader byteReader);
    }
}