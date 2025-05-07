using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
[Route("api/registered_domain")]
public class RegisteredDomainController(IRegisteredDomainRepository registeredDomainRepository) : BaseApiController
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
        return Ok("Xóa sản phẩm domain thành công");
    }
}