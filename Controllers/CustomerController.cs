using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;

public class CustomerController(ICustomerRepository customerRepository,ITokenService tokenService) : BaseApiController
{

    [HttpPost("login")]
    public async Task<ActionResult<CustomerAuthDto>> Login(LoginDto loginDto)
    {
        
        var existedCustomer = await customerRepository.GetByPropertyAsync(x => x.Email == loginDto.Email,x => x.HasType);

        if(existedCustomer == null) return BadRequest("Tên tài khoản sai");
        
        using var hmac = new HMACSHA512(existedCustomer.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        
        if(computedHash.Length != existedCustomer.PasswordHash.Length) return BadRequest("Mật khẩu sai");

        for (int i = 0; i < computedHash.Length; i++)
        {
            if(!computedHash[i].Equals(existedCustomer.PasswordHash[i]))
            {
                return BadRequest("Mật khẩu sai");
            }
        }

        var token = tokenService.CreateCustomerToken(existedCustomer);

        return new CustomerAuthDto{
            Name = existedCustomer.Name,
            Token = token,
            HasType = existedCustomer.HasType.TypeName
        };

    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        return Ok(await customerRepository.GetAllDtoAsync<CustomerDto>());
    }
    [Authorize(Roles = "Quản trị viên")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        CustomerDto customer;
        try
        {
            customer = await customerRepository.GetDtoByIdAsync<CustomerDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        
        return Ok(customer);
    }
    [HttpPost("register")]
    public async Task<ActionResult<CustomerAuthDto>> Register(RegisterCustomerDto registerCustomerDto)
    {
        Customer customer;

        try
        {
            using var hmac = new HMACSHA512();

            registerCustomerDto.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerCustomerDto.Password));

            registerCustomerDto.PasswordSalt = hmac.Key;

            customer = await customerRepository.AddAsync(registerCustomerDto,["Name","Email","PhoneNumber","Address"]);

        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        
        var createdCustomer = await customerRepository.GetByIdAsync(customer.Id,x => x.HasType);

        return Ok(
            new CustomerAuthDto{
                Name = createdCustomer.Name,
                Token = tokenService.CreateCustomerToken(createdCustomer),
                HasType = createdCustomer.HasType.TypeName
            }
        );
        
    }
    [Authorize(Roles = "Quản trị viên,Khách hàng")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCustomer(int id,[FromBody] ChangeCustomerDto updateCustomerDto)
    {

        var customer = await customerRepository.GetByIdAsync(id);

        updateCustomerDto.CustomerTypeId = customer.CustomerTypeId;
        try
        {
            await customerRepository.UpdateAsync(id,updateCustomerDto,["Name","PhoneNumber","Address"]);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa khách hàng thành công");
    }
    [Authorize(Roles = "Quản trị viên")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCustomer(int id)
    {
        try
        {
            await customerRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa khách hàng thành công");
    }
    [Authorize (Roles = "Quản trị viên")]
    [HttpGet("{name}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerByName(string name)
    {
        CustomerDto customer;

        try
        {   
            customer = await customerRepository.GetDtoByPropertyAsync<CustomerDto>(x => x.Name.Equals(name));
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        
        return Ok(customer);
    }
    [Authorize (Roles = "Khách hàng")]
    [HttpGet("profile")]
    public async Task<ActionResult<CustomerDto>> GetCustomerProfile()
    {
        var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");
        
        var customerId = int.Parse(customerNameIndentifier.Value);

        return await customerRepository.GetDtoByIdAsync<CustomerDto>(customerId);
    }
}