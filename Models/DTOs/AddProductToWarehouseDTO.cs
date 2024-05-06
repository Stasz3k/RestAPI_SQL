namespace RestApi_nauka.Models.DTOs
{
    public class AddProductToWarehouseDTO
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
