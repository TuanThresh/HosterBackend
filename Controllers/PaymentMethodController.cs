using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers;

[Route("api/payment_method")]


public class PaymentMethodController(IPaymentMethodRepository paymentMethodRepository) : BaseApiController
{
    [Authorize (Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng,Khách hàng")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetPaymentMethods([FromQuery]PagedListParams pagedListParams)
    {
        var paymentMethods = await paymentMethodRepository.GetAllDtoAsync<PaymentMethodDto>(pagedListParams);

        Response.AddPaginationHeader(paymentMethods);


        return Ok();
    }
    [Authorize (Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng,Khách hàng")]

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
    [Authorize (Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng,Khách hàng")]


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
    [Authorize (Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]


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
    [Authorize (Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]


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