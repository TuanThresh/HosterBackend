using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
[Route("api/domain_product")]
public class DomainProductController(IDomainProductRepository domainProductRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomainProductDto>>> GetDomainProducts()
    {
        return Ok(await domainProductRepository.GetAllDtoAsync<DomainProductDto>());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DomainProductDto>> GetDomainProduct(int id)
    {
        DomainProductDto domainProduct;
        
        try
        {
            domainProduct = await domainProductRepository.GetDtoByIdAsync<DomainProductDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(domainProduct);
    }

    [HttpPost]
    public async Task<ActionResult> CreateDomainProduct(ChangeDomainProductDto domainProductDto)
    {
        try
        {
            await domainProductRepository.AddAsync(domainProductDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo sản phẩm domain thành công");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDomainProduct(int id,[FromBody]ChangeDomainProductDto domainProductDto)
    {
        try
        {
            await domainProductRepository.UpdateAsync(id,domainProductDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa sản phẩm domain thành công");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDomainProduct(int id)
    {
        try
        {
            await domainProductRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa sản phẩm domain thành công");
    }
}