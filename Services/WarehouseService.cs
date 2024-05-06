using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestApi_nauka.Services.Interface;

namespace RestApi_nauka.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IConfiguration _configuration;

        public WarehouseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> DoesProductExist(int productId)
        {
            const string query = "SELECT 1 FROM Product WHERE IdProduct = @ProductId";
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<bool> DoesWarehouseExist(int warehouseId)
        {
            const string query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @WarehouseId";
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@WarehouseId", warehouseId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<bool> ValidateOrder(int productId, int amount)
        {
            const string query = "SELECT 1 FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt <= @Today";
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Today", DateTime.UtcNow); 

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<int> AddProductToWarehouse(int productId, int warehouseId, int amount, int orderId)
        {
            const string insertQuery = @"
        INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, Amount, IdOrder, Price, CreatedAt)
        VALUES (@IdProduct, @IdWarehouse, @Amount, @IdOrder, 
                (SELECT Price FROM Product WHERE IdProduct = @IdProduct) * @Amount, @CreatedAt);
        SELECT SCOPE_IDENTITY();";
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@IdProduct", productId);
            command.Parameters.AddWithValue("@IdWarehouse", warehouseId);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@IdOrder", orderId);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }


        public async Task UpdateOrderFulfilledDate(int orderId)
        {
            const string updateQuery = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @OrderId";
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }



    }
}
