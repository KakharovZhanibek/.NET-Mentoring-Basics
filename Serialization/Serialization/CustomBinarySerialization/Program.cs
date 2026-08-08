using CustomBinarySerialization;
using CustomBinarySerialization.Models;

const string filePath = "product.bin";

// ── Create a sample object ────────────────────────────────────────────────
var product = new ProductRecord("Wireless Keyboard", 49.99m);
Console.WriteLine("Original object:");
Console.WriteLine(product);

// ── Serialize to file using our custom ISerializable mechanism ────────────
using (var writeStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
{
    CustomBinarySerializer.Serialize(product, writeStream);
}
Console.WriteLine($"\nSerialized to '{filePath}' ({new FileInfo(filePath).Length} bytes).");

// ── Deserialize from file ─────────────────────────────────────────────────
ProductRecord deserialized;
using (var readStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
{
    deserialized = CustomBinarySerializer.Deserialize<ProductRecord>(readStream);
}

Console.WriteLine("\nDeserialized object:");
Console.WriteLine(deserialized);

// ── Verify round-trip ─────────────────────────────────────────────────────
bool match = deserialized.ProductName == product.ProductName
          && deserialized.Price       == product.Price;
Console.WriteLine($"\nRound-trip successful: {match}");
