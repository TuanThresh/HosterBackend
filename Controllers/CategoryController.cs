using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers;
[Authorize (Roles = "Nhân viên phòng kinh doanh và tiếp thị")]
public class CategoryController(ICategoryRepository categoryRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery]PagedListParams pagedListParams)
    {
        var categories = await categoryRepository.GetAllDtoAsync<CategoryDto>(pagedListParams);

        Response.AddPaginationHeader(categories);


        return Ok(categories);
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