using System.Threading.Tasks;
using AutoMapper;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
public class RoleController(IRoleRepository roleRepository) : BaseApiController
{
    [Authorize(Roles = "Quản trị viên")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await roleRepository.GetAllDtoAsync<RoleDto>(x => x.GivenEmployees);

        return Ok(roles);
    }
    [Authorize(Roles = "Quản trị viên")]
    [HttpGet("{id:int}")]
    public async Task<RoleDto> GetRole(int id)
    {
        return await roleRepository.GetDtoByIdAsync<RoleDto>(id);
    }
    [Authorize(Roles = "Quản trị viên")]
    [HttpPost]
    public async Task<ActionResult> CreateRole(ChangeRoleDto createRoleDto)
    {
        try
        {
            await roleRepository.AddAsync(createRoleDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }

        return Ok("Tạo vai trò thành công");
    }
    [Authorize(Roles = "Quản trị viên")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateRole(int id,[FromBody] ChangeRoleDto updateRoleDto)
    {
        try
        {
            await roleRepository.UpdateAsync(id,updateRoleDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa vai trò thành công");
    }
    [Authorize(Roles = "Quản trị viên")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        try
        {
            await roleRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa vai trò thành công");
    }
}