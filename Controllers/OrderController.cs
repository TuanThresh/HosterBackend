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

public class OrderController(IOrderRepository orderRepository,ICustomerRepository customerRepository,IDomainProductRepository domainProductRepository,IDomainAccountRepository domainAccountRepository,IRegisteredDomainRepository registeredDomainRepository) : BaseApiController
{
    [Authorize(Roles = "Khách hàng")]
    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        var domainProductToBuy = await domainProductRepository.GetByIdAsync(createOrderDto.DomainProductId);

        var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");
        
        var customerId = int.Parse(customerNameIndentifier.Value);

        createOrderDto.CustomerId = customerId;

        createOrderDto.TotalPrice = domainProductToBuy.Price * createOrderDto.DurationByMonth;


        await orderRepository.AddAsync(createOrderDto);

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
        var orderToUpdate = await orderRepository.GetByIdAsync(id);

        // if (updateOrderDto.Status == OrderStatusEnum.Cancelled ||
        //     updateOrderDto.Status == OrderStatusEnum.Pending ||
        //     (updateOrderDto.Status == OrderStatusEnum.Paid && orderToUpdate.Status == OrderStatusEnum.Paid))
        // {
        //     return Ok("Sửa đơn hàng thành công");
        // }

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

                    var newRegisteredDomain = new RegisteredDomain
                    {
                        FullDomainName = fullDomainName,
                        DomainAccountId = newDomainAccount.Id,
                        DomainProductId = domainProduct.Id,
                        OrderId = orderToUpdate.Id,
                        RegisteredAt = DateTime.Now,
                        ExpiredAt = DateTime.Now.AddMonths(orderToUpdate.DurationByMonth)
                    };
                    Console.WriteLine("Tạo RegisteredDomain");

                    await registeredDomainRepository.AddAsync(newRegisteredDomain);
                    
                    break;

                    
                }
            }

            if(createNewAccount)
            {
                newDomainAccount = new DomainAccount
                {
                    Username = customer.Name + random.Next(100000, 1000000).ToString(),
                    RegisterPanel = "DefaultPanel",
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(GenerateRandomPassword())),
                    PasswordSalt = hmac.Key
                };
                var fullDomainName = $"{orderToUpdate.DomainFirstPart.ToString().ToLower()}.{domainProduct.DomainName.ToString().ToLower()}";

                var newRegisteredDomain = new RegisteredDomain
                {
                    FullDomainName = fullDomainName,
                    DomainAccountId = newDomainAccount.Id,
                    DomainProductId = domainProduct.Id,
                    OrderId = orderToUpdate.Id,
                    RegisteredAt = DateTime.Now,
                    ExpiredAt = DateTime.Now.AddMonths(orderToUpdate.DurationByMonth)
                };
                Console.WriteLine("Tạo RegisteredDomain");

                await registeredDomainRepository.AddAsync(newRegisteredDomain);
                
                Console.WriteLine("Tạo domain account");
                // await domainAccountRepository.AddAsync(newDomainAccount);
            }

            
        }
        else
        {
            var registeredDomain = await registeredDomainRepository.GetByPropertyAsync(x => x.FullDomainName == $"{orderToUpdate.DomainFirstPart}.{domainProduct.DomainName}");

            registeredDomain = RenewRegisteredDomain(registeredDomain,orderToUpdate.DurationByMonth);

            Console.WriteLine("Cập nhật RegisteredDomain");

            await registeredDomainRepository.UpdateAsync(registeredDomain.Id,registeredDomain);
        }
    }
    catch (Exception ex)
    {
        return BadRequest(ex);
    }

    return Ok("Sửa đơn hàng thành công");
}

}