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
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace HosterBackend.Controllers
{
    public class EmployeeController(IEmployeeRepository employeeRepository,IAuthorizeRepository authorizeRepository,IMapper mapper,ITokenService tokenService) : BaseApiController
    {

        [HttpPost("login")]
        public async Task<ActionResult<EmployeeAuthDto>> Login(LoginDto loginDto)
        {
            var existedEmployee = await employeeRepository.GetEmployeeByEmailOrName("",loginDto.Name);

            if(existedEmployee == null) return BadRequest("Tên tài khoản sai");
            
            using var hmac = new HMACSHA512(existedEmployee.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            
            if(computedHash.Length != existedEmployee.PasswordHash.Length) return BadRequest("Mật khẩu sai");

            for (int i = 0; i < computedHash.Length; i++)
            {
                if(!computedHash[i].Equals(existedEmployee.PasswordHash[i]))
                {
                    return BadRequest("Mật khẩu sai");
                }
            }

            var token = tokenService.CreateEmployeeToken(existedEmployee);

            var roles = existedEmployee.HasRoles.Select(x => x.Role.RoleName).ToList();

            
            return new EmployeeAuthDto{
                Name = existedEmployee.Name,
                Token = token,
                HasRoles = roles
            };

        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            return Ok(await employeeRepository.GetAllDtoAsync<EmployeeDto>());
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
        public async Task<ActionResult> CreateEmployee(ChangeEmployeeDto createEmployeeDto)
        {
            try
            {   
                using var hmac = new HMACSHA512();

                createEmployeeDto.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(createEmployeeDto.Password));

                createEmployeeDto.PasswordSalt = hmac.Key;

                var employee = await employeeRepository.AddAsync(createEmployeeDto);

                if(createEmployeeDto.Roles.Count > 0)
                {
                    foreach (var roleId in createEmployeeDto.Roles)
                    {
                        authorizeRepository.AuthorizeEmployee(employee.Id,roleId);
                    }
                }

                else authorizeRepository.AuthorizeEmployee(employee.Id,1);

                await authorizeRepository.SaveAllAsync();
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok("Tạo nhân viên thành công");
        }
        [Authorize(Roles = "Quản trị viên")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateEmployee(int id,[FromBody] ChangeEmployeeDto updateEmployeeDto)
        {
            try
            {  
                var existedEmployee = await employeeRepository.GetByIdAsync(id);
                 
                mapper.Map(updateEmployeeDto,existedEmployee);

                using var hmac = new HMACSHA512();

                existedEmployee.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(updateEmployeeDto.Password));

                existedEmployee.PasswordSalt = hmac.Key;

                await employeeRepository.UpdateAsync(id,existedEmployee);

                var existedAuthorizes = await authorizeRepository.GetAuthorizes(id);

                if(existedAuthorizes.Any())
                {
                    foreach (var authorize in existedAuthorizes)
                    {
                        authorizeRepository.DeleteAuthorize(authorize);
                    }
                }

                if(updateEmployeeDto.Roles.Count > 0)
                {
                    foreach (var roleId in updateEmployeeDto.Roles)
                    {
                        authorizeRepository.AuthorizeEmployee(existedEmployee.Id,roleId);
                    }
                }
                await authorizeRepository.SaveAllAsync();
                
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
        
    }
}