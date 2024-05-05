using Tutorial6.Models.DTOs;

namespace Tutorial6.Repositories;

public interface IWarehouseRepository
{
    //Task<bool> DoesProductWarehouseExist(WarehouseProductDTO warehouseProductDto);
    Task<bool> DoesIdProductExist(int idProduct);
    Task<bool> DoesWareHouseExist(int idWarehouse);
    Task<bool> IsAmountGreaterThan(int amount);
    Task<bool> AddProduct(WarehouseProductDTO warehouseProductDto);
    Task<bool> CheckOrder(int id);
    Task<bool> UpdateOrder(int id);
    Task<int> AddToWarehouse(WarehouseProductDTO warehouseProductDto);
    Task<int> ExecuteSQLProcedure(WarehouseProductDTO warehouseProductDto);

}