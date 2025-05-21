using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;

public class CustomerController(ICustomerRepository customerRepository, ITokenService tokenService, IPasswordResetTokenRepository passwordResetTokenRepository, IMailService mailService) : BaseApiController
{

    [HttpPost("login")]
    public async Task<ActionResult<CustomerAuthDto>> Login(LoginDto loginDto)
    {

        var existedCustomer = await customerRepository.GetByPropertyAsync(x => x.Email == loginDto.Email, x => x.HasType);

        if (existedCustomer == null) return BadRequest("Tên tài khoản sai");

        using var hmac = new HMACSHA512(existedCustomer.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        if (computedHash.Length != existedCustomer.PasswordHash.Length) return BadRequest("Mật khẩu sai");

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (!computedHash[i].Equals(existedCustomer.PasswordHash[i]))
            {
                return BadRequest("Mật khẩu sai");
            }
        }

        var token = tokenService.CreateCustomerToken(existedCustomer);

        return new CustomerAuthDto
        {
            Name = existedCustomer.Name,
            Token = token,
            HasType = existedCustomer.HasType.TypeName
        };

    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị,Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        return Ok(await customerRepository.GetAllDtoAsync<CustomerDto>());
    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị,Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]
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

            customer = await customerRepository.AddAsync(registerCustomerDto, ["Name", "Email", "PhoneNumber", "Address"]);

        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        var createdCustomer = await customerRepository.GetByIdAsync(customer.Id, x => x.HasType);

        return Ok(
            new CustomerAuthDto
            {
                Name = createdCustomer.Name,
                Token = tokenService.CreateCustomerToken(createdCustomer),
                HasType = createdCustomer.HasType.TypeName
            }
        );

    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị,Nhân viên phòng kỹ thuật hỗ trợ khách hàng, Khách hàng")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCustomer(int id, [FromBody] ChangeCustomerDto updateCustomerDto)
    {

        var customer = await customerRepository.GetByIdAsync(id);

        updateCustomerDto.CustomerTypeId = customer.CustomerTypeId;
        try
        {
            await customerRepository.UpdateAsync(id, updateCustomerDto, ["Name", "PhoneNumber", "Address"]);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa khách hàng thành công");
    }
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị,Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]
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
    [Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị,Nhân viên phòng kỹ thuật hỗ trợ khách hàng")]
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
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<CustomerDto>> GetCustomerProfile()
    {
        var customerNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");

        var customerId = int.Parse(customerNameIndentifier.Value);

        return await customerRepository.GetDtoByIdAsync<CustomerDto>(customerId);
    }
    [Authorize]
    [HttpPost("forgot_password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var customer = await customerRepository.GetByPropertyAsync(x => x.Email.Equals(forgotPasswordDto.Email));

            if (customer == null) return BadRequest("Không tìm thấy người dùng với email");

            var token = await passwordResetTokenRepository.GetByPropertyAsync(x => x.Email == forgotPasswordDto.Email
                        && !x.IsUsed
                        && x.ExpiredAt > DateTime.Now);

            if (token == null)
            {
                token = new PasswordResetToken
                {
                    Email = forgotPasswordDto.Email,
                    Token = Guid.NewGuid().ToString(),
                    ExpiredAt = DateTime.Now.AddMinutes(30)
                };

                await passwordResetTokenRepository.AddAsync(token);
            }

            await mailService.SendForgotPasswordEmaiAsync(forgotPasswordDto.Email, "Quên mật khẩu", token.Token, "Customer");
        }
        catch (Exception ex)
        {

            return BadRequest(ex);
        }

        return Ok("Đã gửi hướng dẫn mật khẩu tới email");

    }
    [Authorize]
    [HttpPost("reset_password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var customer = await customerRepository.GetByPropertyAsync(x => x.Email.Equals(resetPasswordDto.Email));

            if (customer == null) return BadRequest("Không tìm thấy người dùng với email");

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
                return BadRequest("Mật khẩu mới không khớp.");

            var token = await passwordResetTokenRepository.GetByPropertyAsync(x => x.Email == resetPasswordDto.Email
                        && x.Token == resetPasswordDto.Token
                        && !x.IsUsed
                        && x.ExpiredAt > DateTime.Now);

            if (token == null)
                return BadRequest("Token không hợp lệ hoặc đã hết hạn");

            token.IsUsed = true;

            await passwordResetTokenRepository.UpdateAsync(token.Id, token);

            using var hmac = new HMACSHA512();

            customer.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resetPasswordDto.NewPassword));

            customer.PasswordSalt = hmac.Key;

            await customerRepository.UpdateAsync(customer.Id, customer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok("Đặt lại mật khẩu thành công");
    }
        [Authorize]

        [HttpPut("change_password")]
        public async Task<ActionResult> ChangePassword( [FromBody] ChangePasswordDto changePasswordDto)
        {
        try
        {
            var nameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");

            var id = int.Parse(nameIndentifier.Value);

            var customer = await customerRepository.GetByIdAsync(id);

            using var hmac = new HMACSHA512(customer.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.CurrentPassword));

            if (computedHash.Length != customer.PasswordHash.Length) return BadRequest("Mật khẩu hiện tại sai");

            for (int i = 0; i < customer.PasswordHash.Length; i++)
            {
                if (customer.PasswordHash[i] != computedHash[i]) return BadRequest("Mật khẩu hiện tại sai");
            }

            customer.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.NewPassword));

            customer.PasswordSalt = hmac.Key;

            await customerRepository.UpdateAsync(customer.Id, customer);
            }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
            return Ok("Sửa mật khẩu thành công");
        }
        [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]
        [HttpPost("statistic")]
        public async Task<ActionResult> GetStatistic(StatisticConditionDto statisticConditionDto,[FromQuery] string CustomerTypeId = "1")
        {
            var customers = await customerRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= statisticConditionDto.From &&
                DateOnly.FromDateTime(x.CreatedAt) <= statisticConditionDto.To &&
                x.CustomerTypeId == int.Parse(CustomerTypeId));

            return Ok(customers.Count());
        }
        [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]


        [HttpGet("overview")]
        public async Task<ActionResult> GetOverview([FromQuery] string CustomerTypeId = "1")
        {
            List<int> counts = [];
            for (int i = 1; i <= 12; i++)
            {

                var startDate = new DateOnly(2025, i, 1);

                var endDate = startDate.AddMonths(1).AddDays(-1);

                var customers = await customerRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= startDate &&
                DateOnly.FromDateTime(x.CreatedAt) <= endDate &&
                x.CustomerTypeId == int.Parse(CustomerTypeId));

                counts.Add(customers.Count());

            }

            return Ok(counts);
        }
    }