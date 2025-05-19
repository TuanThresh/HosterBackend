using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
[Route("api/registered_domain")]
public class RegisteredDomainController(IRegisteredDomainRepository registeredDomainRepository,IDomainProductRepository domainProductRepository,IOrderRepository orderRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegisteredDomainDto>>> GetRegisteredDomains()
    {
        return Ok(await registeredDomainRepository.GetAllDtoAsync<RegisteredDomainDto>());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegisteredDomainDto>> GetRegisteredDomain(int id)
    {
        RegisteredDomainDto registeredDomain;
        
        try
        {
            registeredDomain = await registeredDomainRepository.GetDtoByIdAsync<RegisteredDomainDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(registeredDomain);
    }

    [HttpGet("check_domain")]
    public async Task<ActionResult<RegisteredDomainDto>> CheckDomain(CheckDomainDto checkDomainDto)
    {
        var domainProduct = await domainProductRepository.GetByIdAsync(checkDomainDto.DomainProductId);

        var fullDomainName = $"{checkDomainDto.DomainFirstPart.ToString().ToLower()}.{domainProduct.DomainName.ToString().ToLower()}";
        
        if(await orderRepository.CheckExistsAsync(x => x.DomainProductId == checkDomainDto.DomainProductId && x.DomainFirstPart.Equals(checkDomainDto.DomainFirstPart.ToString().ToLower()))) return Ok("Tên miền đã tồn tại");


        if (await registeredDomainRepository.CheckExistsAsync(x => x.FullDomainName == fullDomainName)) return Ok("Tên miền đã tồn tại");

        return Ok("Tên miền chưa tồn tại");
    }

    

    // [HttpPut("{id:int}")]
    // public async Task<ActionResult> UpdateRegisteredDomain(int id,[FromBody]ChangeRegisteredDomainDto RegisteredDomainDto)
    // {
    //     try
    //     {
    //         await RegisteredDomainRepository.UpdateAsync(id,RegisteredDomainDto);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex);
    //     }
    //     return Ok("Sửa sản phẩm domain thành công");
    // }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteRegisteredDomain(int id)
    {
        try
        {
            await registeredDomainRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa tên miền đã đăng kí thành công");
    }
}