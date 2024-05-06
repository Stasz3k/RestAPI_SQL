using Microsoft.AspNetCore.Mvc;
using RestApi_nauka.Models;

namespace RestApi_nauka.Services.Interface
{
    public interface IWarehouseService
    {
        Task<bool> DoesProductExist(int productId);
        Task<bool> DoesWarehouseExist(int warehouseId);
        Task<bool> ValidateOrder(int productId, int amount);
        Task<int> AddProductToWarehouse(int productId, int warehouseId, int amount, int orderId);
        Task UpdateOrderFulfilledDate(int orderId);
    }
}
