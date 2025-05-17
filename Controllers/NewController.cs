using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
public class NewController(INewRepository newRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewDto>>> GetNews()
    {
        return Ok(await newRepository.GetAllDtoAsync<NewDto>());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NewDto>> GetNew(int id)
    {
        NewDto New;
        
        try
        {
            New = await newRepository.GetDtoByIdAsync<NewDto>(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok(New);
    }

    [HttpPost]
    public async Task<ActionResult> CreateNew([FromForm]ChangeNewDto newDto)
    {
        try
        {
            if (newDto.Image == null || newDto.Image.Length == 0) return BadRequest("Ảnh không hợp lệ");

            var uploadsFolder = Path.Combine("wwwroot", "news-images");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(newDto.Image.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);
    
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newDto.Image.CopyToAsync(stream);
            }

            var imageUrl = $"/news-images/{fileName}";

            newDto.FileName = fileName;

            newDto.FilePath = filePath;

            newDto.ImageUrl = imageUrl;
            

            await newRepository.AddAsync(newDto);
            
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Tạo tin tức thành công");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateNew(int id,[FromBody]ChangeNewDto newDto)
    {
        try
        {
            if (newDto.Image == null || newDto.Image.Length == 0) return BadRequest("Ảnh không hợp lệ");

            var uploadsFolder = Path.Combine("wwwroot", "news-images");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            if (System.IO.File.Exists(newDto.FilePath))
            {
                System.IO.File.Delete(newDto.FilePath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(newDto.Image.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);
    
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await newDto.Image.CopyToAsync(stream);
            }

            var imageUrl = $"/news-images/{fileName}";

            newDto.FileName = fileName;

            newDto.FilePath = filePath;

            newDto.ImageUrl = imageUrl;

            await newRepository.UpdateAsync(id, newDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Sửa tin tức thành công");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteNew(int id)
    {
        try
        {
            var news = await newRepository.GetByIdAsync(id);

            if (System.IO.File.Exists(news.FilePath))
            {
                System.IO.File.Delete(news.FilePath);
            }

            await newRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
        return Ok("Xóa tin tức thành công");
    }
}