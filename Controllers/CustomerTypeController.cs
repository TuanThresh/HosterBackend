using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers;

[Route("api/customer_type")]
[Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]

public class CustomerTypeController(ICustomerTypeRepository customerTypeRepository) : BaseApiController
{
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerTypeDto>>> GetCustomerTypes([FromQuery]PagedListParams pagedListParams)
    {
        var customerTypes = await customerTypeRepository.GetAllDtoAsync<CustomerTypeDto>(pagedListParams, x => x.HasCustomers);

        Response.AddPaginationHeader(customerTypes);

        return Ok(customerTypes);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerTypeDto>> GetCustomerType(int id)
    {
        CustomerTypeDto customerType;
        
        try
        {
            customerType = await customerTypeRepository.GetDtoByIdAsync<CustomerTypeDto>(id,x => x.HasCustomers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(customerType);
    }
    [HttpPost]
    public async Task<ActionResult> CreateCustomerType(ChangeCustomerTypeDto createCusomerTypeDto)
    {
        try
        {
            await customerTypeRepository.AddAsync(createCusomerTypeDto,["TypeName"]);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo loại khách hàng thành công");
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCusomerType(int id,[FromBody]ChangeCustomerTypeDto updateCustomerTypeDto)
    {
        try
        {
            await customerTypeRepository.UpdateAsync(id,updateCustomerTypeDto,["TypeName"]);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa loại khách hàng thành công");
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCustomerType(int id)
    {
        try
        {
            await customerTypeRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa loại khách hàng thành công");
    }

}