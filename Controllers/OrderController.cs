using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using HosterBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;

public class OrderController(IOrderRepository orderRepository,
ICustomerRepository customerRepository,
IDomainProductRepository domainProductRepository,
IDomainAccountRepository domainAccountRepository,
IRegisteredDomainRepository registeredDomainRepository,
IDiscountRepository discountRepository,
IMailService mailService) : BaseApiController
{
    [Authorize(Roles = "Khách hàng")]
    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        try
        {
            var domainProduct = await domainProductRepository.GetByIdAsync(createOrderDto.DomainProductId);

            var fullDomainName = $"{createOrderDto.DomainFirstPart.ToString().ToLower()}.{domainProduct.DomainName.ToString().ToLower()}";

            var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");

            var customerId = int.Parse(customerNameIndentifier.Value);

            if(await orderRepository.CheckExistsAsync(x => x.DomainFirstPart == createOrderDto.DomainFirstPart && x.DomainProductId == createOrderDto.DomainProductId && x.Status == OrderStatusEnum.Pending)) return BadRequest("Tên miền đang được chờ phê duyệt mua");

            if(await registeredDomainRepository.CheckExistsAsync(x => x.FullDomainName.Equals(fullDomainName) && x.Order.CustomerId != customerId)) return BadRequest("Tên miền đã tồn tại");

            var domainProductToBuy = await domainProductRepository.GetByIdAsync(createOrderDto.DomainProductId);

            var customer = await customerRepository.GetByIdAsync(customerId,x => x.HasType,x => x.Orders);

            if (createOrderDto.DiscountCode != null)
            {
                var discount = await discountRepository.GetByPropertyAsync(x => x.DiscountCode.Equals(createOrderDto.DiscountCode)) ?? throw new Exception("Không tìm thấy mã giảm giá");

                if(discount.ExpiredAt < DateTime.Now) return BadRequest("Mã giảm giá đã hết hạn");

                if(await orderRepository.CheckExistsAsync(x => x.CustomerId == customerId && x.DiscountId == discount.Id)) return BadRequest("Mã giảm giá đã được khách hàng sử dụng rồi");

                createOrderDto.TotalPrice = (int)Math.Round(domainProductToBuy.Price * createOrderDto.DurationByMonth * (100 - discount.Percentage) / 100.0);
                
            }
            else createOrderDto.TotalPrice = domainProductToBuy.Price * createOrderDto.DurationByMonth;

            createOrderDto.CustomerId = customerId;

            // createOrderDto.TotalPrice = (int)Math.Round(domainProductToBuy.Price * createOrderDto.DurationByMonth * (100 - discount.Percentage) / 100.0);

            var order = await orderRepository.AddAsync(createOrderDto);

            var createdOrder = await orderRepository.GetByIdAsync(order.Id,x => x.Customer,x => x.DomainProduct,x => x.PaymentMethod);

            

            await mailService.SendEmailAsync(customer.Email,"Mua tên miền",order);

        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo order thành công");

    }
    
    private static RegisteredDomain RenewRegisteredDomain(RegisteredDomain registeredDomain,int DurationByMonth)
    {
        if (registeredDomain.ExpiredAt > DateTime.Now)
            {
                registeredDomain.ExpiredAt = registeredDomain.ExpiredAt.AddMonths(DurationByMonth);
            }
        else
        {
            registeredDomain.ExpiredAt = DateTime.Now.AddMonths(DurationByMonth);
        }

        registeredDomain.UpdatedAt = DateTime.Now;

        return registeredDomain;
    }

    private static string GenerateRandomPassword(int length = 12)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";

        var random = new Random();

        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        if(User.Claims.Where(x => x.Type == ClaimTypes.Role).Any(c => c.Value == "Quản trị viên"))
        {
            return Ok(await orderRepository.GetDtoByIdAsync<OrderDto>(id));
        }
        else if(User.Claims.Where(x => x.Type == ClaimTypes.Role).Any(c => c.Value == "Khách hàng"))
        {
            var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");
        
            var customerId = int.Parse(customerNameIndentifier.Value);

            return Ok(await orderRepository.GetDtoByPropertyAsync<OrderDto>(x => x.CustomerId == customerId));
        }
        return Unauthorized("Không có quyền truy cập");  
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        if(User.Claims.Where(x => x.Type == ClaimTypes.Role).Any(c => c.Value == "Quản trị viên"))
        {
            return Ok(await orderRepository.GetAllDtoAsync<OrderDto>());
        }
        else if(User.Claims.Where(x => x.Type == ClaimTypes.Role).Any(c => c.Value == "Khách hàng"))
        {
            var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");
        
            var customerId = int.Parse(customerNameIndentifier.Value);

            return Ok(await orderRepository.GetAllDtoByPropertyAsync<OrderDto>(x => x.CustomerId == customerId));
        }
        return Unauthorized("Không có quyền truy cập");  
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
        try
        {
            await orderRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa đơn hàng thành công");
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
{
    try
    {
        var orderToUpdate = await orderRepository.GetByIdAsync(id,x => x.Customer,x => x.DomainProduct,x => x.PaymentMethod);

        if(orderToUpdate.Status == OrderStatusEnum.Cancelled) return BadRequest("Đơn hàng đã bị hủy");

        else if(updateOrderDto.Status == OrderStatusEnum.Pending) return BadRequest("Không thể cập nhật trạng thái đang chờ cho đơn hàng");

        else if (updateOrderDto.Status == OrderStatusEnum.Paid && orderToUpdate.Status == OrderStatusEnum.Paid) return BadRequest("Đơn hàng đã được thanh toán rồi");

        await orderRepository.UpdateAsync(id, updateOrderDto);

        var customer = await customerRepository.GetByIdAsync(orderToUpdate.CustomerId);
        
        var domainProduct = await domainProductRepository.GetByIdAsync(orderToUpdate.DomainProductId);

        if (await registeredDomainRepository.CheckExistsAsync(x => x.FullDomainName == $"{orderToUpdate.DomainFirstPart}.{domainProduct.DomainName}") == false)
        {

            var random = new Random();

            using var hmac = new HMACSHA512();

            var createNewAccount = true;

            DomainAccount newDomainAccount;

            foreach(var domainAccount in await domainAccountRepository.GetAllAsync(x => x.RegisteredDomains))
            {
                if(domainAccount.RegisteredDomains.Any(x => x.DomainProductId == orderToUpdate.DomainProductId))
                {
                    createNewAccount = false;

                    newDomainAccount = domainAccount;

                    var fullDomainName = $"{orderToUpdate.DomainFirstPart.ToString().ToLower()}.{domainProduct.DomainName.ToString().ToLower()}";

                    await CreateRegisteredDomain(fullDomainName,newDomainAccount,domainProduct,orderToUpdate);
                    
                    break;

                    
                }
            }

            if(createNewAccount)
            {
                var randomPassword = GenerateRandomPassword();
                var username = customer.Name + random.Next(100000, 1000000).ToString();
                newDomainAccount = new DomainAccount
                {
                    Username = username,
                    RegisterPanel = "DefaultPanel",
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(randomPassword)),
                    PasswordSalt = hmac.Key
                };

                var fullDomainName = $"{orderToUpdate.DomainFirstPart.ToString().ToLower()}.{domainProduct.DomainName.ToString().ToLower()}";

                newDomainAccount = await domainAccountRepository.AddAsync(newDomainAccount);

                await CreateRegisteredDomain(fullDomainName,newDomainAccount,domainProduct,orderToUpdate);

                await mailService.SendEmailAsync(orderToUpdate.Customer.Email,"Mua tên miền",orderToUpdate,username,randomPassword);
            }

            
        }
        else
        {
            var registeredDomain = await registeredDomainRepository.GetByPropertyAsync(x => x.FullDomainName == $"{orderToUpdate.DomainFirstPart}.{domainProduct.DomainName}") ?? throw new Exception("Không tìm thấy miền đã đăng ký");

            registeredDomain = RenewRegisteredDomain(registeredDomain,orderToUpdate.DurationByMonth);

            await registeredDomainRepository.UpdateAsync(registeredDomain.Id,registeredDomain);

            await mailService.SendEmailAsync(orderToUpdate.Customer.Email,"Mua tên miền",orderToUpdate);
        }

    }
    catch (Exception ex)
    {
        return BadRequest(ex);
    }

    return Ok("Sửa đơn hàng thành công");
}
    private async Task CreateRegisteredDomain(string fullDomainName,DomainAccount newDomainAccount,DomainProduct domainProduct,Order orderToUpdate)
    {
        var newRegisteredDomain = new RegisteredDomain
        {
            FullDomainName = fullDomainName,
            DomainAccountId = newDomainAccount.Id,
            DomainProductId = domainProduct.Id,
            OrderId = orderToUpdate.Id,
            RegisteredAt = DateTime.Now,
            ExpiredAt = DateTime.Now.AddMonths(orderToUpdate.DurationByMonth)
        };

        await registeredDomainRepository.AddAsync(newRegisteredDomain);
    }

}
