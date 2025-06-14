using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers
{
    public class EmployeeController(IEmployeeRepository employeeRepository,
    IAuthorizeRepository authorizeRepository,
    IMapper mapper, ITokenService tokenService,
    IMailService mailService,
    IPasswordResetTokenRepository passwordResetTokenRepository) : BaseApiController
    {

        [HttpPost("login")]
        public async Task<ActionResult<EmployeeAuthDto>> Login(LoginDto loginDto)
        {
            var existedEmployee = await employeeRepository.GetEmployeeByEmailOrName(loginDto.Email, "");

            if (existedEmployee == null) return BadRequest("Tên tài khoản sai");

            if (existedEmployee.Status == Data.Enums.EmployeeStatusEnum.ChoXacThuc) return BadRequest("Tài khoản chưa được xác thực");

            if (existedEmployee.Status == Data.Enums.EmployeeStatusEnum.Khoa) return BadRequest("Tài khoản đã bị khóa");

            using var hmac = new HMACSHA512(existedEmployee.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            if (computedHash.Length != existedEmployee.PasswordHash.Length) return BadRequest("Mật khẩu sai");

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (!computedHash[i].Equals(existedEmployee.PasswordHash[i]))
                {
                    return BadRequest("Mật khẩu sai");
                }
            }

            var token = tokenService.CreateEmployeeToken(existedEmployee);

            var roles = existedEmployee.HasRoles.Select(x => x.Role.RoleName).ToList();


            return new EmployeeAuthDto
            {
                Name = existedEmployee.Name,
                Token = token,
                HasRoles = roles
            };

        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees([FromQuery]PagedListParams pagedListParams)
        {

            var employeeNameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của khách hàng");

            var employeeId = int.Parse(employeeNameIndentifier.Value);

            var employees = await employeeRepository.GetAllDtoByPropertyAsync<EmployeeDto>(pagedListParams, x => x.Id != employeeId);

            Response.AddPaginationHeader(employees);

            return Ok(employees);
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            EmployeeDto employee;
            try
            {
                employee = await employeeRepository.GetDtoByIdAsync<EmployeeDto>(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(employee);
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                using var hmac = new HMACSHA512();

                var randomPassword = GenerateRandomPassword();

                createEmployeeDto.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(randomPassword));

                createEmployeeDto.PasswordSalt = hmac.Key;

                var employee = await employeeRepository.AddAsync(createEmployeeDto, ["Email", "Name", "Address"]);

                var createdEmployee = await employeeRepository.GetByIdAsync(employee.Id, x => x.HasRoles);

                await authorizeRepository.AuthorizeEmployee(employee.Id, 1);

                await mailService.SendCreatedEmployee("Chào mừng nhân viên mới", employee, randomPassword);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok("Tạo nhân viên thành công");
        }
        private static string GenerateRandomPassword(int length = 12)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";

            var random = new Random();

            return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var existedEmployee = await employeeRepository.GetByIdAsync(id);

                mapper.Map(updateEmployeeDto, existedEmployee);

                using var hmac = new HMACSHA512();

                await employeeRepository.UpdateAsync(id, existedEmployee, ["Name", "PhoneNumber", "Address"]);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok("Sửa nhân viên thành công");
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            try
            {
                await employeeRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok("Xóa nhân viên thành công");
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpPost("authorize")]
        public async Task<ActionResult> AuthorizeEmployee([FromBody] AuthorizeEmployeeDto authorizeEmployeeDto)
        {
            var existedEmployee = await employeeRepository.GetByIdAsync(authorizeEmployeeDto.EmployeeId, x => x.HasRoles);

            if (existedEmployee.HasRoles.Any(x => x.RoleId == authorizeEmployeeDto.RoleId)) return BadRequest("Nhân viên đã có quyền này rồi");

            await authorizeRepository.AuthorizeEmployee(existedEmployee.Id, authorizeEmployeeDto.RoleId);

            return Ok("Phân quyền thành công");
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpDelete("remove_authorize")]
        public async Task<ActionResult> RemoveAuthorizeEmployee([FromBody] AuthorizeEmployeeDto authorizeEmployeeDto)
        {
            try
            {
                var authorize = await authorizeRepository.GetAuthorize(authorizeEmployeeDto.EmployeeId, authorizeEmployeeDto.RoleId);
                await authorizeRepository.DeleteAuthorize(authorize);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok("Hủy phân quyền thành công");
        }
        [HttpPost("forgot_password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var employee = await employeeRepository.GetByPropertyAsync(x => x.Email.Equals(forgotPasswordDto.Email));

                if (employee == null) return BadRequest("Không tìm thấy người dùng với email");

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

                await mailService.SendForgotPasswordEmaiAsync(forgotPasswordDto.Email, "Quên mật khẩu", token.Token, "Employee");
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

            return Ok("Đã gửi hướng dẫn mật khẩu tới email");

        }
        [HttpPost("reset_password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var employee = await employeeRepository.GetByPropertyAsync(x => x.Email.Equals(resetPasswordDto.Email));

                if (employee == null) return BadRequest("Không tìm thấy người dùng với email");

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

                employee.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resetPasswordDto.NewPassword));

                employee.PasswordSalt = hmac.Key;

                await employeeRepository.UpdateAsync(employee.Id, employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok("Đặt lại mật khẩu thành công");
        }
        [HttpGet("{name}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByName(string name)
        {
            EmployeeDto employee;

            try
            {
                employee = await employeeRepository.GetDtoByPropertyAsync<EmployeeDto>(x => x.Name.Equals(name.ToLower()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(employee);
        }
        [HttpPut("change_password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var nameIndentifier = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("Không tìm thấy Id của nhân viên");

                var id = int.Parse(nameIndentifier.Value);

                var employee = await employeeRepository.GetByIdAsync(id);

                using var hmac = new HMACSHA512(employee.PasswordSalt);

                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.CurrentPassword));

                if (computedHash.Length != employee.PasswordHash.Length) return BadRequest("Mật khẩu hiện tại sai");

                for (int i = 0; i < employee.PasswordHash.Length; i++)
                {
                    if (employee.PasswordHash[i] != computedHash[i]) return BadRequest("Mật khẩu hiện tại sai");
                }

                employee.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.NewPassword));

                employee.PasswordSalt = hmac.Key;

                await employeeRepository.UpdateAsync(employee.Id,employee);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok("Sửa mật khẩu thành công");
        }
        [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]

        [HttpPost("statistic")]
        public async Task<ActionResult> GetStatistic(StatisticConditionDto statisticConditionDto,[FromQuery] EmployeeStatusEnum employeeStatusEnum = EmployeeStatusEnum.KichHoat)
        {
            var employees = await employeeRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= statisticConditionDto.From &&
                DateOnly.FromDateTime(x.CreatedAt) <= statisticConditionDto.To &&
                x.Status == employeeStatusEnum);

            return Ok(employees.Count());
        }
        [Authorize (Roles = "Nhân viên phòng tài chính và kế toán")]

        [HttpGet("overview")]
        public async Task<ActionResult> GetOverview([FromQuery] EmployeeStatusEnum employeeStatusEnum = EmployeeStatusEnum.KichHoat)
        {
            List<int> counts = [];
            for (int i = 1; i <= 12; i++)
            {

                var startDate = new DateOnly(2025, i, 1);

                var endDate = startDate.AddMonths(1).AddDays(-1);

                var employees = await employeeRepository.GetAllByPropertyAsync(x =>
                DateOnly.FromDateTime(x.CreatedAt) >= startDate &&
                DateOnly.FromDateTime(x.CreatedAt) <= endDate &&
                x.Status == employeeStatusEnum);

                counts.Add(employees.Count());

            }

            return Ok(counts);
        }
    }
}