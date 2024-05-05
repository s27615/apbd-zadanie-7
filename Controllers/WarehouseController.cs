using Microsoft.AspNetCore.Mvc;
using Tutorial6.Models.DTOs;
using Tutorial6.Repositories;

namespace Tutorial6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : Controller
{
    private readonly IWarehouseRepository _warehouseRepository;
    
    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    // Popraw trasę, aby była bardziej RESTful i jasna
    [HttpPost("DoesProductExistInWarehouse")]
    public async Task<IActionResult> DoesProductWarehouseExist([FromBody] WarehouseProductDTO warehouseProductDto)
    {
        if (!await _warehouseRepository.DoesIdProductExist(warehouseProductDto.IdProduct))
            return NotFound("Nie znaleziono produktu");
        
        if (!await _warehouseRepository.DoesWareHouseExist(warehouseProductDto.IdWarehouse))
            return NotFound("Nie znaleziono magazynu");

        if (!await _warehouseRepository.IsAmountGreaterThan(warehouseProductDto.Amount))
        {
            return BadRequest("Błąd: Ilość mniejsza niż 0");
        }
            
        return Ok();
    }

    [HttpGet("IsOrderRealised/{idOrder}")]
    public async Task<IActionResult> IsRealised(int idOrder)
    {
        if (await _warehouseRepository.CheckOrder(idOrder))
            return Ok("Zamówienie już dodane");
            
        return Ok("Brak zamówienia");
    }
    
    [HttpGet("UpdateOrderDate/{idOrder}")]
    public async Task<IActionResult> UpdateDate(int idOrder)
    {
        if (!await _warehouseRepository.UpdateOrder(idOrder))
             return BadRequest("Data nie została zaktualizowana");
            
        return Ok("Polecenie zostało wykonane");
    }

    [HttpPost("AddProductToWarehouse")]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] WarehouseProductDTO warehouseProductDto)
    {
        if (!await _warehouseRepository.AddProduct(warehouseProductDto))
            return BadRequest("Nie dodano produktu");
            
        return Ok();
    }
    
    [HttpPost("AddToWarehouse")]
    public async Task<IActionResult> AddToWarehouse([FromBody] WarehouseProductDTO warehouseProductDto)
    {
        if (await _warehouseRepository.AddToWarehouse(warehouseProductDto) == 0)
            return BadRequest("Nie dodano produktu");
            
        return Ok(await _warehouseRepository.AddToWarehouse(warehouseProductDto));
    }
    
    [HttpPost("ExecuteProcderue")]
    public async Task<IActionResult> ExcecuteSQLProcedure([FromBody] WarehouseProductDTO warehouseProductDto)
    {
        if (await _warehouseRepository.ExecuteSQLProcedure(warehouseProductDto) == 0)
            return BadRequest("Nie dodano produktu");
            
        return Ok(await _warehouseRepository.AddToWarehouse(warehouseProductDto));
    }
}
