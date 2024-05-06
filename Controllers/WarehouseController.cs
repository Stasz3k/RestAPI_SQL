using Microsoft.AspNetCore.Mvc;
using RestApi_nauka.Services.Interface;
using System.Threading.Tasks;

namespace RestApi_nauka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost("addProductToWarehouse")]
        public async Task<IActionResult> AddProductToWarehouse(int productId, int warehouseId, int amount, int orderId)
        {
            if (!await _warehouseService.DoesProductExist(productId) || !await _warehouseService.DoesWarehouseExist(warehouseId))
            {
                return NotFound("Product or warehouse does not exist.");
            }

            if (!await _warehouseService.ValidateOrder(productId, amount))
            {
                return BadRequest("No valid order found for this product and amount.");
            }

            int newEntryId = await _warehouseService.AddProductToWarehouse(productId, warehouseId, amount, orderId);
            await _warehouseService.UpdateOrderFulfilledDate(orderId);  // Update using the orderId

            return Ok($"New entry created with ID: {newEntryId}");
        }


    }
}
