using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;



[Route("api/domain_product")]
public class DomainProductController(IDomainProductRepository domainProductRepository) : BaseApiController
{
    [Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng,Khách hàng")]

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomainProductDto>>> GetDomainProducts()
    {
        return Ok(await domainProductRepository.GetAllDtoAsync<DomainProductDto>());
    }
    [Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng,Khách hàng")]

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
    [Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]

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
    [Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDomainProduct(int id, [FromBody] ChangeDomainProductDto domainProductDto)
    {
        try
        {
            await domainProductRepository.UpdateAsync(id, domainProductDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa sản phẩm domain thành công");
    }
    [Authorize(Roles = "Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]

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
            [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]

    
    [HttpPost("statistic")]
        public async Task<ActionResult> GetStatistic(StatisticConditionDto statisticConditionDto,[FromQuery] DomainTypeEnum domainTypeEnum= DomainTypeEnum.VietNam)
        {
            var employees = await domainProductRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= statisticConditionDto.From &&
                DateOnly.FromDateTime(x.CreatedAt) <= statisticConditionDto.To &&
                x.DomainType == domainTypeEnum);

            return Ok(employees.Count());
        }
            [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]

        [HttpGet("overview")]
        public async Task<ActionResult> GetOverview([FromQuery] DomainTypeEnum domainTypeEnum = DomainTypeEnum.VietNam)
        {
            List<int> counts = [];
            for (int i = 1; i <= 12; i++)
            {

                var startDate = new DateOnly(2025, i, 1);

                var endDate = startDate.AddMonths(1).AddDays(-1);

                var domainProducts = await domainProductRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= startDate &&
                DateOnly.FromDateTime(x.CreatedAt) <= endDate &&
                x.DomainType == domainTypeEnum);

                counts.Add(domainProducts.Count());

            }

            return Ok(counts);
        }
}