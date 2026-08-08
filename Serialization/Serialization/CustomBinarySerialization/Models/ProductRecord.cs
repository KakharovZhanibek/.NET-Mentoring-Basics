using System.Runtime.Serialization;

namespace CustomBinarySerialization.Models;

/// <summary>
/// A simple class with two properties that implements <see cref="ISerializable"/>
/// to provide a fully custom binary serialization mechanism.
///
/// Because BinaryFormatter was removed in .NET 9 we wire the ISerializable
/// contract to our own <see cref="BinaryWriter"/>/<see cref="BinaryReader"/>
/// helper.  The class still honours the full ISerializable pattern:
///   • GetObjectData  – writes fields into a SerializationInfo bag
///   • Deserialization constructor – restores fields from that bag
/// </summary>
[Serializable]
public class ProductRecord : ISerializable
{
    // ?? Properties ???????????????????????????????????????????????????????????
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }

    // ?? Normal constructor ????????????????????????????????????????????????????
    public ProductRecord(string productName, decimal price)
    {
        ProductName = productName;
        Price = price;
    }

    // ?? ISerializable: deserialization constructor ????????????????????????????
    /// <summary>
    /// Called by the serialization infrastructure to restore an instance.
    /// Values are read from the <see cref="SerializationInfo"/> bag that was
    /// populated by <see cref="GetObjectData"/>.
    /// </summary>
    protected ProductRecord(SerializationInfo info, StreamingContext context)
    {
        ProductName = info.GetString(nameof(ProductName))!;
        Price       = info.GetDecimal(nameof(Price));
    }

    // ?? ISerializable: serialization ??????????????????????????????????????????
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> bag with the data needed to
    /// recreate this instance.
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(ProductName), ProductName);
        info.AddValue(nameof(Price),       Price);
    }

    public override string ToString() =>
        $"ProductRecord {{ ProductName = \"{ProductName}\", Price = {Price:C} }}";
}
