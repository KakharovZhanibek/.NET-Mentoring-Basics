using System.Data;
using Microsoft.Data.SqlClient;
using Store.DAL.Enums;
using Store.DAL.Models;

namespace Store.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "SELECT Id, Status, CreatedDate, UpdatedDate, ProductId FROM [Order] WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapOrder(reader) : null;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(
            int? month = null, int? year = null, OrderStatus? status = null, int? productId = null)
        {
            var orders = new List<Order>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("GetOrders", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
            command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", status.HasValue ? (object)status.Value.ToString() : DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", (object?)productId ?? DBNull.Value);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                orders.Add(MapOrder(reader));
            return orders;
        }

        public async Task<int> CreateAsync(Order order)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "INSERT INTO [Order] (Status, CreatedDate, UpdatedDate, ProductId) " +
                "VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);",
                connection);
            command.Parameters.AddWithValue("@Status", order.Status.ToString());
            command.Parameters.AddWithValue("@CreatedDate", order.CreatedDate);
            command.Parameters.AddWithValue("@UpdatedDate", order.UpdatedDate);
            command.Parameters.AddWithValue("@ProductId", order.ProductId);
            var result = await command.ExecuteScalarAsync();
            return result is not null and not DBNull ? Convert.ToInt32(result) : 0;
        }

        public async Task UpdateAsync(Order order)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand(
                "UPDATE [Order] SET Status = @Status, UpdatedDate = @UpdatedDate, ProductId = @ProductId WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@Status", order.Status.ToString());
            command.Parameters.AddWithValue("@UpdatedDate", order.UpdatedDate);
            command.Parameters.AddWithValue("@ProductId", order.ProductId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DELETE FROM [Order] WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteBulkAsync(
            int? month = null, int? year = null, OrderStatus? status = null, int? productId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("DeleteOrders", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
            command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", status.HasValue ? (object)status.Value.ToString() : DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", (object?)productId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        private static Order MapOrder(SqlDataReader reader) => new()
        {
            Id = reader.GetInt32("Id"),
            Status = Enum.Parse<OrderStatus>(reader.GetString("Status")),
            CreatedDate = reader.GetDateTime("CreatedDate"),
            UpdatedDate = reader.GetDateTime("UpdatedDate"),
            ProductId = reader.GetInt32("ProductId")
        };
    }
}
