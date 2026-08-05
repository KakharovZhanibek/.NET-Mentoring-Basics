using Microsoft.Data.SqlClient;

namespace Store.DAL.Tests.Infrastructure
{
    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }

    public class DatabaseFixture : IDisposable
    {
        private const string MasterConnection = "Server=(localdb)\\mssqllocaldb;Database=master;Integrated Security=true;TrustServerCertificate=True;";
        private const string DbName = "StoreDalTests";
        public string ConnectionString { get; } = "Server=(localdb)\\mssqllocaldb;Database=StoreDalTests;Integrated Security=true;TrustServerCertificate=True;";

        public DatabaseFixture()
        {
            DropAndCreateDatabase();
            CreateSchema();
        }

        private void DropAndCreateDatabase()
        {
            SqlConnection.ClearAllPools();

            using (var con = new SqlConnection(MasterConnection))
            {
                con.Open();
                using var checkCmd = con.CreateCommand();
                checkCmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = 'StoreDalTests'";
                var exists = (int)checkCmd.ExecuteScalar()! > 0;

                if (exists)
                {
                    using var dropCmd = con.CreateCommand();
                    dropCmd.CommandText = "ALTER DATABASE [StoreDalTests] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [StoreDalTests]";
                    dropCmd.ExecuteNonQuery();
                }
            }

            SqlConnection.ClearAllPools();

            using (var con = new SqlConnection(MasterConnection))
            {
                con.Open();
                using var createCmd = con.CreateCommand();
                createCmd.CommandText = "CREATE DATABASE [StoreDalTests]";
                createCmd.ExecuteNonQuery();
            }
        }

        private void CreateSchema()
        {
            using var con = new SqlConnection(ConnectionString);
            con.Open();

            ExecuteNonQuery(con,
                "CREATE TABLE Product (" +
                "Id INT IDENTITY(1,1) PRIMARY KEY," +
                "Name NVARCHAR(200) NOT NULL," +
                "Description NVARCHAR(1000) NULL," +
                "Weight DECIMAL(18,4) NOT NULL," +
                "Height DECIMAL(18,4) NOT NULL," +
                "Width DECIMAL(18,4) NOT NULL," +
                "Length DECIMAL(18,4) NOT NULL);");

            ExecuteNonQuery(con,
                "CREATE TABLE [Order] (" +
                "Id INT IDENTITY(1,1) PRIMARY KEY," +
                "Status NVARCHAR(50) NOT NULL," +
                "CreatedDate DATETIME2 NOT NULL," +
                "UpdatedDate DATETIME2 NOT NULL," +
                "ProductId INT NOT NULL," +
                "CONSTRAINT FK_Order_Product FOREIGN KEY (ProductId) REFERENCES Product(Id));");

            ExecuteNonQuery(con,
                "CREATE PROCEDURE GetOrders @Month INT = NULL, @Year INT = NULL, @Status NVARCHAR(50) = NULL, @ProductId INT = NULL AS BEGIN SET NOCOUNT ON; SELECT Id, Status, CreatedDate, UpdatedDate, ProductId FROM [Order] WHERE (@Month IS NULL OR MONTH(CreatedDate) = @Month) AND (@Year IS NULL OR YEAR(CreatedDate) = @Year) AND (@Status IS NULL OR Status = @Status) AND (@ProductId IS NULL OR ProductId = @ProductId); END;");

            ExecuteNonQuery(con,
                "CREATE PROCEDURE DeleteOrders @Month INT = NULL, @Year INT = NULL, @Status NVARCHAR(50) = NULL, @ProductId INT = NULL AS BEGIN SET NOCOUNT ON; DELETE FROM [Order] WHERE (@Month IS NULL OR MONTH(CreatedDate) = @Month) AND (@Year IS NULL OR YEAR(CreatedDate) = @Year) AND (@Status IS NULL OR Status = @Status) AND (@ProductId IS NULL OR ProductId = @ProductId); END;");
        }

        public void ResetTables()
        {
            using var con = new SqlConnection(ConnectionString);
            con.Open();
            ExecuteNonQuery(con, "DELETE FROM [Order]");
            ExecuteNonQuery(con, "DELETE FROM Product");
        }

        private static void ExecuteNonQuery(SqlConnection con, string sql)
        {
            using var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            SqlConnection.ClearAllPools();
            using var con = new SqlConnection(MasterConnection);
            con.Open();
            using var cmd = con.CreateCommand();
            cmd.CommandText = "ALTER DATABASE [StoreDalTests] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [StoreDalTests]";
            cmd.ExecuteNonQuery();
        }
    }
}
