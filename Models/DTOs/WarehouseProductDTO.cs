namespace Tutorial6.Models.DTOs;

public class WarehouseProductDTO
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int Amount { get; set; }
    public int IdOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public double Price { get; set;}
}