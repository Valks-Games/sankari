using ENet;

using System.IO;
using System.Reflection;

namespace Sankari.Netcode;

public class PacketReader : IDisposable
{
    private readonly MemoryStream _stream;
    private readonly BinaryReader _reader;
    private readonly byte[] ReadBuffer = new byte[GamePacket.MaxSize];

    public PacketReader(Packet packet)
    {
        _stream = new MemoryStream(ReadBuffer);
        _reader = new BinaryReader(_stream);
        packet.CopyTo(ReadBuffer);
        packet.Dispose();
    }

    public byte ReadByte() => _reader.ReadByte();
    public sbyte ReadSByte() => _reader.ReadSByte();
    public char ReadChar() => _reader.ReadChar();
    public string ReadString() => _reader.ReadString();
    public bool ReadBool() => _reader.ReadBoolean();
    public short ReadShort() => _reader.ReadInt16();
    public ushort ReadUShort() => _reader.ReadUInt16();
    public int ReadInt() => _reader.ReadInt32();
    public uint ReadUInt() => _reader.ReadUInt32();
    public float ReadFloat() => _reader.ReadSingle();
    public double ReadDouble() => _reader.ReadDouble();
    public long ReadLong() => _reader.ReadInt64();
    public ulong ReadULong() => _reader.ReadUInt64();
    public byte[] ReadBytes(int count) => _reader.ReadBytes(count);
    public byte[] ReadBytes() => ReadBytes(ReadInt());

    public Vector2 ReadVector2() =>
        new(ReadFloat(), ReadFloat());

    public dynamic Read(Type t)
    {
        if (t == typeof(byte)) return ReadByte();
        if (t == typeof(sbyte)) return ReadSByte();
        if (t == typeof(char)) return ReadChar();
        if (t == typeof(string)) return ReadString();
        if (t == typeof(bool)) return ReadBool();
        if (t == typeof(short)) return ReadShort();
        if (t == typeof(ushort)) return ReadUShort();
        if (t == typeof(int)) return ReadInt();
        if (t == typeof(uint)) return ReadUInt();
        if (t == typeof(float)) return ReadFloat();
        if (t == typeof(double)) return ReadDouble();
        if (t == typeof(long)) return ReadLong();
        if (t == typeof(ulong)) return ReadULong();
        if (t == typeof(byte[])) return ReadBytes();
        if (t == typeof(Vector2)) return ReadVector2();
        if (t == typeof(Guid)) return new Guid(ReadBytes());

        if (t.IsGenericType)
        {
            var g = t.GetGenericTypeDefinition();

            if (g == typeof(IList<>) || g == typeof(List<>))
            {
                var vt = t.GetGenericArguments()[0];

                var count = ReadInt();

                dynamic list = Activator
                    .CreateInstance(typeof(List<>)
                    .MakeGenericType(vt));

                for (var i = 0; i < count; i++)
                    list.Add(Read(vt));

                return list;
            }

            if (g == typeof(IDictionary<,>) || g == typeof(Dictionary<,>))
            {
                var kt = t.GetGenericArguments()[0];
                var vt = t.GetGenericArguments()[1];

                var count = ReadInt();

                dynamic dict = Activator
                    .CreateInstance(typeof(Dictionary<,>)
                    .MakeGenericType(kt, vt));

                for (var i = 0; i < count; i++)
                    dict.Add(Read(kt), Read(vt));

                return dict;
            }
        }

        if (t.IsEnum)
        {
            var v = ReadByte();
            return Enum.ToObject(t, v);
        }

        if (t.IsValueType)
        {
            var v = Activator.CreateInstance(t);

            var fields = t
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(field => field.MetadataToken);

            foreach (var f in fields)
                f.SetValue(v, Read(f.FieldType));

            return v;
        }

        throw new NotImplementedException("PacketReader: " + t + " is not a supported type.");
    }

    public T Read<T>() =>
        Read(typeof(T));

    public void Dispose()
    {
        _stream.Dispose();
        _reader.Dispose();
    }
}
