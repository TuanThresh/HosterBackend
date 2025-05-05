using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;

public class DiscountController(IDiscountRepository discountRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetDiscounts()
    {
        return Ok(await discountRepository.GetAllDtoAsync<DiscountDto>());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiscountDto>> GetDiscount(int id)
    {
        DiscountDto Discount;
        
        try
        {
            Discount = await discountRepository.GetDtoByIdAsync<DiscountDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(Discount);
    }

    [HttpPost]
    public async Task<ActionResult> CreateDiscount(ChangeDiscountDto DiscountDto)
    {
        try
        {
            await discountRepository.AddAsync(DiscountDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo mã giảm giá thành công");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDiscount(int id,[FromBody]ChangeDiscountDto DiscountDto)
    {
        try
        {
            await discountRepository.UpdateAsync(id,DiscountDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa mã giảm giá thành công");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDiscount(int id)
    {
        try
        {
            await discountRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa mã giảm giá thành công");
    }
}