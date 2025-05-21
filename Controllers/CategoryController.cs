using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
[Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]
public class CategoryController(ICategoryRepository categoryRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategorys()
    {
        return Ok(await categoryRepository.GetAllDtoAsync<CategoryDto>());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        CategoryDto category;
        
        try
        {
            category = await categoryRepository.GetDtoByIdAsync<CategoryDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(ChangeCategoryDto categoryDto)
    {
        try
        {
            await categoryRepository.AddAsync(categoryDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo danh mục thành công");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCategory(int id,[FromBody]ChangeCategoryDto categoryDto)
    {
        try
        {
            await categoryRepository.UpdateAsync(id,categoryDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa danh mục thành công");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        try
        {
            await categoryRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa danh mục thành công");
    }
}