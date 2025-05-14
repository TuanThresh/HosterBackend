using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;

public class DiscountController(IDiscountRepository discountRepository,IMailService mailService,ICustomerRepository customerRepository) : BaseApiController
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
    public async Task<ActionResult> CreateDiscount(ChangeDiscountDto createDiscountDto)
    {
        try
        {
            var discount = await discountRepository.AddAsync(createDiscountDto);

            var customers = await customerRepository.GetAllAsync();
            
            if(createDiscountDto.CustomerTypeId != null)
            {
                customers = await customerRepository.GetAllByPropertyAsync(x => x.CustomerTypeId == createDiscountDto.CustomerTypeId);
            }
            foreach (var customer in customers)
            {
                await mailService.SendDiscountCodeEmailAsync(customer.Email,"Gửi mã giảm giá cho khách hàng",discount);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo mã giảm giá thành công");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDiscount(int id,[FromBody]ChangeDiscountDto changeDiscountDto)
    {
        try
        {
            await discountRepository.UpdateAsync(id,changeDiscountDto);
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