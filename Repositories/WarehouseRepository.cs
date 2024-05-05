using System.Data;
using Tutorial6.Models.DTOs;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Tutorial6.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // public Task<bool> DoesProductWarehouseExist(WarehouseProductDTO warehouseProductDto)
    // {
    //     throw new NotImplementedException();
    //     return Task.FromResult(true);
    // }

    public async Task<bool> DoesIdProductExist(int idProduct)
    {
        
        var query = "SELECT 1 FROM Product WHERE IdProduct = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idProduct);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    
    public async Task<bool> DoesWareHouseExist(int idWarehouse)
    {
        
        var query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idWarehouse);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    

    public Task<bool> IsAmountGreaterThan(int amount)
    {
        if(amount > 0)
            return Task.FromResult(true);
        else
            return Task.FromResult(false);
    }

    public async Task<bool> AddProduct(WarehouseProductDTO warehouseProductDto)
    {
        var query = "SELECT 1 FROM [Order] WHERE IdOrder = @ID AND IdProduct = @IDPRODUCT AND Amount = @AMOUNT AND CreatedAt < @CREATEDAT";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
        using SqlCommand command = new SqlCommand();
    
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", warehouseProductDto.IdOrder);
        command.Parameters.AddWithValue("@IDPRODUCT", warehouseProductDto.IdProduct);
        command.Parameters.AddWithValue("@AMOUNT", warehouseProductDto.Amount);
        command.Parameters.AddWithValue("@CREATEDAT", warehouseProductDto.CreatedAt);
    
        await connection.OpenAsync();
    
        var res = await command.ExecuteScalarAsync();
        if (res == null)
            return false;
        else
        {
            query = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IDWAREHOUSE, @IDPRODUCT, @ID, @AMOUNT, @PRICE, @CREATEDAT)";
            using SqlConnection connectionI = new SqlConnection(_configuration.GetConnectionString("Default"));
        
            using SqlCommand commandI = new SqlCommand();
    
            commandI.Connection = connectionI;
            commandI.CommandText = query;
            commandI.Parameters.AddWithValue("@ID", warehouseProductDto.IdOrder);
            commandI.Parameters.AddWithValue("@IDPRODUCT", warehouseProductDto.IdProduct);
            commandI.Parameters.AddWithValue("@AMOUNT", warehouseProductDto.Amount);
            commandI.Parameters.AddWithValue("@IDWAREHOUSE", warehouseProductDto.IdWarehouse);
            commandI.Parameters.AddWithValue("@PRICE", warehouseProductDto.Price);
            commandI.Parameters.AddWithValue("@CREATEDAT", warehouseProductDto.CreatedAt);
            await connectionI.OpenAsync();
            await commandI.ExecuteNonQueryAsync();
            return true;
        }
        
    }

    public async Task<bool> CheckOrder(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<bool> UpdateOrder(int id)
    {
        var query = "UPDATE [ORDER] SET FulfilledAt = getdate() where IdOrder = @ID";
        using SqlConnection connectionI = new SqlConnection(_configuration.GetConnectionString("Default"));
        
        using SqlCommand commandI = new SqlCommand();

        commandI.Connection = connectionI;
        commandI.CommandText = query;
        commandI.Parameters.AddWithValue("@ID", id);
        
        await connectionI.OpenAsync();
        await commandI.ExecuteNonQueryAsync();
        return true;
    }
    
    public async Task<int> AddToWarehouse(WarehouseProductDTO warehouseProductDto)
    {
        var query = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IDWAREHOUSE, @IDPRODUCT, @ID, @AMOUNT, ((select price from product where idproduct = @IDPRODUCT) * (select amount from [order] where idorder = @ID)), @CREATEDAT)";
            using SqlConnection connectionI = new SqlConnection(_configuration.GetConnectionString("Default"));
        
            using SqlCommand commandI = new SqlCommand();
    
            commandI.Connection = connectionI;
            commandI.CommandText = query;
            commandI.Parameters.AddWithValue("@ID", warehouseProductDto.IdOrder);
            commandI.Parameters.AddWithValue("@IDPRODUCT", warehouseProductDto.IdProduct);
            commandI.Parameters.AddWithValue("@AMOUNT", warehouseProductDto.Amount);
            commandI.Parameters.AddWithValue("@IDWAREHOUSE", warehouseProductDto.IdWarehouse);
            commandI.Parameters.AddWithValue("@CREATEDAT", warehouseProductDto.CreatedAt);
            await connectionI.OpenAsync();
            await commandI.ExecuteNonQueryAsync();
            
            
            query = "SELECT top 1 idProductWareHouse FROM Product_WareHouse order by idProductWareHouse desc ";
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        
            using SqlCommand command = new SqlCommand();
    
            command.Connection = connection;
            command.CommandText = query;
            await connection.OpenAsync();
            object resultObj = await command.ExecuteScalarAsync();

            if (resultObj != null)
            {
                int result = Convert.ToInt32(resultObj);
                return result;
            }
            return 0;
        
    }
    public async Task<int> ExecuteSQLProcedure(WarehouseProductDTO warehouseProductDto)
    {
        using SqlConnection connectionI = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand commandI = new SqlCommand("AddProductToWarehouse", connectionI);
        commandI.CommandType = CommandType.StoredProcedure;
        commandI.Parameters.AddWithValue("@IdProduct", warehouseProductDto.IdProduct);
        commandI.Parameters.AddWithValue("@IdWarehouse", warehouseProductDto.IdWarehouse);
        commandI.Parameters.AddWithValue("@Amount", warehouseProductDto.Amount);
        commandI.Parameters.AddWithValue("@CreatedAt", warehouseProductDto.CreatedAt);

        SqlDataAdapter da = new SqlDataAdapter(commandI);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count == 0)
            return 0;
        return Convert.ToInt32(dt.Rows[0]["NewId"]);
    }

    // public Task<bool> DoesWareHouseExist(int idWarehouse)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task<bool> IsAmountGreaterThan(int amount)
    // {
    //     throw new NotImplementedException();
    // }
}