using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Controllers;
public class AuthorizeController(IAuthorizeRepository authorizeRepository) : BaseApiController
{
[Authorize(Roles = "Quản trị viên")]

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorizeDto>>> GetAuthorizes()
    {
        return Ok(await authorizeRepository.GetAuthorizeDtos());
    }

}