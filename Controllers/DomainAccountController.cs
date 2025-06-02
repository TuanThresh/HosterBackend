using System.Security.Cryptography;
using System.Text;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers;
[Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]

[Route("api/domain_account")]
public class DomainAccountController(IDomainAccountRepository domainAccountRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomainAccountDto>>> GetDomainAccounts([FromQuery]PagedListParams pagedListParams)
    {
        var domainAccounts = await domainAccountRepository.GetAllDtoAsync<DomainAccountDto>(pagedListParams);

        Response.AddPaginationHeader(domainAccounts);
        

        return Ok(domainAccounts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DomainAccountDto>> GetDomainAccount(int id)
    {
        DomainAccountDto domainAccount;
        
        try
        {
            domainAccount = await domainAccountRepository.GetDtoByIdAsync<DomainAccountDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(domainAccount);
    }

    // [HttpPost]
    // public async Task<ActionResult> CreateDomainAccount(ChangeDomainAccountDto domainAccountDto)
    // {
    //     try
    //     {
    //         using var hmac  = new HMACSHA512();

    //         if (domainAccountDto.Password == null) return BadRequest("");

    //         domainAccountDto.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(domainAccountDto.Password));

    //         domainAccountDto.PasswordSalt = hmac.Key;

    //         await domainAccountRepository.AddAsync(domainAccountDto);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex);
    //     }
    //     return Ok("Tạo tài khoản domain thành công");
    // }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDomainAccount(int id,[FromBody]ChangeDomainAccountDto domainAccountDto)
    {
        try
        {
            using var hmac  = new HMACSHA512();

            // domainAccountDto.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(domainAccountDto.Password));

            // domainAccountDto.PasswordSalt = hmac.Key;
            
            await domainAccountRepository.UpdateAsync(id,domainAccountDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa tài khoản domain thành công");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDomainAccount(int id)
    {
        try
        {
            await domainAccountRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa tài khoản domain thành công");
    }
}