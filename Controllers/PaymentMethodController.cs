using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
[Route("api/payment_method")]
public class PaymentMethodController(IPaymentMethodRepository paymentMethodRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
    {
        return Ok(await paymentMethodRepository.GetAllDtoAsync<PaymentMethodDto>());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentMethodDto>> GetPaymentMethod(int id)
    {
        PaymentMethodDto PaymentMethod;
        
        try
        {
            PaymentMethod = await paymentMethodRepository.GetDtoByIdAsync<PaymentMethodDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(PaymentMethod);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePaymentMethod(ChangePaymentMethodDto PaymentMethodDto)
    {
        try
        {
            await paymentMethodRepository.AddAsync(PaymentMethodDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo phương thức thanh toán thành công");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePaymentMethod(int id,[FromBody]ChangePaymentMethodDto PaymentMethodDto)
    {
        try
        {
            await paymentMethodRepository.UpdateAsync(id,PaymentMethodDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa phương thức thanh toán thành công");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePaymentMethod(int id)
    {
        try
        {
            await paymentMethodRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa phương thức thanh toán thành công");
    }
}