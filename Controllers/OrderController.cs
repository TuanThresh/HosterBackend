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

public class OrderController(IOrderRepository orderRepository,ICustomerRepository customerRepository,IDomainProductRepository domainProductRepository,IDomainAccountRepository domainAccountRepository) : BaseApiController
{
    [Authorize(Roles = "Khách hàng")]
    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        var domainProductToBuy = await domainProductRepository.GetByIdAsync(createOrderDto.DomainProductId);

        var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");
        
        var customerId = int.Parse(customerNameIndentifier.Value);

        var createNewAccount = true;

        var customer = await customerRepository.GetByIdAsync(customerId,x => x.Orders);

        if(customer.Orders.Count > 0)
        {
            
            DomainProduct domainProduct;

            DomainAccount domainAccount;

            foreach (var customerOrder in customer.Orders)
            {
                var order = await orderRepository.GetByIdAsync(customerOrder.Id,x => x.DomainProduct);
                
                if(await orderRepository.GetByPropertyAsync(x => x.DomainProductId == createOrderDto.DomainProductId) != null)
                {
                    domainProduct = await domainProductRepository.GetByIdAsync(order.DomainProductId,x => x.HasAccounts);

                    domainAccount = await domainAccountRepository.GetByPropertyAsync(x => x.DomainProductId == domainProduct.Id && x.Username == customer.Name);

                    domainAccount = RenewExistingAccount(domainAccount,createOrderDto.DurationByMonth);

                    await domainAccountRepository.UpdateAsync(domainAccount.Id,domainAccount);

                    createNewAccount = false;

                    break;
                }
                
            }
        }
        if(createNewAccount)
        {
            using var hmac = new HMACSHA512();

                    var newDomainAccount = new DomainAccount{
                        DomainProductId = domainProductToBuy.Id,
                        RegisterPanel = "DefaultPanel",
                        Username = customer.Name,
                        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(GenerateRandomPassword())),
                        PasswordSalt = hmac.Key,
                        ExpiredAt = DateTime.Now.AddMonths(createOrderDto.DurationByMonth)
                    };

                    Console.WriteLine(newDomainAccount.ExpiredAt);

                    await domainAccountRepository.AddAsync(newDomainAccount);
        }

        createOrderDto.CustomerId = customerId;

        createOrderDto.TotalPrice = domainProductToBuy.Price * createOrderDto.DurationByMonth;

        await orderRepository.AddAsync(createOrderDto);

        return Ok("Tạo order thành công");

    }
    
    private static DomainAccount RenewExistingAccount(DomainAccount domainAccount,int DurationByMonth)
    {
        if (domainAccount.ExpiredAt > DateTime.Now)
            {
                domainAccount.ExpiredAt = domainAccount.ExpiredAt.AddMonths(DurationByMonth);
            }
        else
        {
            domainAccount.ExpiredAt = DateTime.Now.AddMonths(DurationByMonth);
        }

        domainAccount.UpdatedAt = DateTime.Now;

        return domainAccount;
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
    public async Task<ActionResult> UpdateOrder(int id,[FromBody] UpdateOrderDto updateOrderDto)
    {
        try
        {
            await orderRepository.UpdateAsync(id,updateOrderDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa đơn hàng thành công");
    }

}