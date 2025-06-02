using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers;



public class DiscountController(IDiscountRepository discountRepository, IMailService mailService, ICustomerRepository customerRepository) : BaseApiController
{
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetDiscounts([FromQuery]PagedListParams pagedListParams)
    {
        var discounts = await discountRepository.GetAllDtoAsync<DiscountDto>(pagedListParams);

        Response.AddPaginationHeader(discounts);


        return Ok(discounts);
    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]
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
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]

    [HttpPost]
    public async Task<ActionResult> CreateDiscount(ChangeDiscountDto createDiscountDto)
    {
        try
        {
            var discount = await discountRepository.AddAsync(createDiscountDto);

            var customers = await customerRepository.GetAllAsync();

            if (createDiscountDto.CustomerTypeId != null)
            {
                customers = await customerRepository.GetAllByPropertyAsync(x => x.CustomerTypeId == createDiscountDto.CustomerTypeId);
            }
            foreach (var customer in customers)
            {
                await mailService.SendDiscountCodeEmailAsync(customer.Email, "Gửi mã giảm giá cho khách hàng", discount);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo mã giảm giá thành công");
    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateDiscount(int id, [FromBody] ChangeDiscountDto changeDiscountDto)
    {
        try
        {
            await discountRepository.UpdateAsync(id, changeDiscountDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa mã giảm giá thành công");
    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]

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
            [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]

    
    [HttpPost("statistic")]
        public async Task<ActionResult> GetStatistic(StatisticConditionDto statisticConditionDto,[FromQuery] string CustomerTypeId = "0")
        {
            var discounts = await discountRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= statisticConditionDto.From &&
                DateOnly.FromDateTime(x.CreatedAt) <= statisticConditionDto.To &&
                (CustomerTypeId != "0" ? x.CustomerTypeId == int.Parse(CustomerTypeId) : x.CustomerTypeId == null));

            return Ok(discounts.Count());
        }
            [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]

        [HttpGet("overview")]
        public async Task<ActionResult> GetOverview([FromQuery] string CustomerTypeId = "0")
        {
            List<int> counts = [];
            for (int i = 1; i <= 12; i++)
            {

                var startDate = new DateOnly(2025, i, 1);

                var endDate = startDate.AddMonths(1).AddDays(-1);

                var discounts = await discountRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= startDate &&
                DateOnly.FromDateTime(x.CreatedAt) <= endDate &&
                (CustomerTypeId != "0" ? x.CustomerTypeId == int.Parse(CustomerTypeId) : x.CustomerType == null));

            Console.WriteLine(CustomerTypeId == "0");

                counts.Add(discounts.Count());

            }

            return Ok(counts);
        }
}