using System.Runtime.Serialization;

namespace CustomBinarySerialization;

/// <summary>
/// Bridges the <see cref="ISerializable"/> / <see cref="SerializationInfo"/>
/// contract to raw <see cref="BinaryWriter"/> / <see cref="BinaryReader"/>
/// streams.
///
/// Supported field types: string, int, long, float, double, decimal, bool.
/// </summary>
public static class CustomBinarySerializer
{
    // ?? Serialize ?????????????????????????????????????????????????????????????
    public static void Serialize<T>(T obj, Stream stream) where T : ISerializable
    {
        var info = new SerializationInfo(typeof(T), new FormatterConverter());
        obj.GetObjectData(info, default);

        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        // Enumerate via the explicit GetEnumerator (SerializationInfo is not IEnumerable<T>).
        var entries = new List<SerializationEntry>();
        foreach (SerializationEntry entry in info)
            entries.Add(entry);

        writer.Write(entries.Count);

        foreach (var entry in entries)
        {
            writer.Write(entry.Name);
            WriteValue(writer, entry.Value);
        }
    }

    // ?? Deserialize ???????????????????????????????????????????????????????????
    public static T Deserialize<T>(Stream stream) where T : ISerializable
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        int count = reader.ReadInt32();
        var info  = new SerializationInfo(typeof(T), new FormatterConverter());

        for (int i = 0; i < count; i++)
        {
            string key   = reader.ReadString();
            object value = ReadValue(reader);
            info.AddValue(key, value);
        }

        // Invoke the ISerializable deserialization constructor via reflection.
        var ctor = typeof(T).GetConstructor(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public,
            binder: null,
            types: [typeof(SerializationInfo), typeof(StreamingContext)],
            modifiers: null)
            ?? throw new InvalidOperationException(
                $"Type {typeof(T).Name} does not have a (SerializationInfo, StreamingContext) constructor.");

        return (T)ctor.Invoke([info, default(StreamingContext)]);
    }

    // ?? Private helpers ???????????????????????????????????????????????????????

    // Type tags written before each value so the reader knows how to decode it.
    private enum TypeTag : byte { String, Int32, Int64, Single, Double, Decimal, Boolean }

    private static void WriteValue(BinaryWriter w, object? value)
    {
        switch (value)
        {
            case string  v: w.Write((byte)TypeTag.String);  w.Write(v); break;
            case int     v: w.Write((byte)TypeTag.Int32);   w.Write(v); break;
            case long    v: w.Write((byte)TypeTag.Int64);   w.Write(v); break;
            case float   v: w.Write((byte)TypeTag.Single);  w.Write(v); break;
            case double  v: w.Write((byte)TypeTag.Double);  w.Write(v); break;
            case decimal v: w.Write((byte)TypeTag.Decimal); w.Write(v); break;
            case bool    v: w.Write((byte)TypeTag.Boolean); w.Write(v); break;
            default:
                throw new NotSupportedException(
                    $"Type '{value?.GetType().Name}' is not supported by {nameof(CustomBinarySerializer)}.");
        }
    }

    private static object ReadValue(BinaryReader r)
    {
        var tag = (TypeTag)r.ReadByte();
        return tag switch
        {
            TypeTag.String  => r.ReadString(),
            TypeTag.Int32   => r.ReadInt32(),
            TypeTag.Int64   => r.ReadInt64(),
            TypeTag.Single  => r.ReadSingle(),
            TypeTag.Double  => r.ReadDouble(),
            TypeTag.Decimal => r.ReadDecimal(),
            TypeTag.Boolean => r.ReadBoolean(),
            _ => throw new NotSupportedException($"Unknown type tag: {tag}")
        };
    }
}
