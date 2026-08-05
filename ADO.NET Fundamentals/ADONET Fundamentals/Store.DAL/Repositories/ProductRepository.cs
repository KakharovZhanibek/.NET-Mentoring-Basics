using System.Data;
using Microsoft.Data.SqlClient;
using Store.DAL.Models;

namespace Store.DAL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name, Description, Weight, Height, Width, Length FROM Product", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                products.Add(MapProduct(reader));
            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT Id, Name, Description, Weight, Height, Width, Length FROM Product WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapProduct(reader) : null;
        }

        public async Task<int> CreateAsync(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO Product (Name, Description, Weight, Height, Width, Length) OUTPUT INSERTED.Id VALUES (@Name, @Description, @Weight, @Height, @Width, @Length)",
                connection);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", product.Weight);
            command.Parameters.AddWithValue("@Height", product.Height);
            command.Parameters.AddWithValue("@Width", product.Width);
            command.Parameters.AddWithValue("@Length", product.Length);
            var result = await command.ExecuteScalarAsync();
            return (int)result!;
        }

        public async Task UpdateAsync(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE Product SET Name = @Name, Description = @Description, Weight = @Weight, Height = @Height, Width = @Width, Length = @Length WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", product.Weight);
            command.Parameters.AddWithValue("@Height", product.Height);
            command.Parameters.AddWithValue("@Width", product.Width);
            command.Parameters.AddWithValue("@Length", product.Length);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM Product WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        private static Product MapProduct(SqlDataReader reader) => new()
        {
            Id = reader.GetInt32("Id"),
            Name = reader.GetString("Name"),
            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
            Weight = reader.GetDecimal("Weight"),
            Height = reader.GetDecimal("Height"),
            Width = reader.GetDecimal("Width"),
            Length = reader.GetDecimal("Length")
        };
    }
}
