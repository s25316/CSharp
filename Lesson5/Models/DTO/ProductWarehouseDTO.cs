namespace Lesson5.Models.DTO
{
    public class ProductWarehouseDTO
    {
        public int IdProduct { get; init; }

        public int IdWarehouse { get; init; }

        public int Amount { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}
