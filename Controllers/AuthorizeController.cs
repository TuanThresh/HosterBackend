using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HosterBackend.Extensions;
namespace HosterBackend.Controllers;
public class AuthorizeController(IAuthorizeRepository authorizeRepository) : BaseApiController
{
[Authorize(Roles = "Quản trị viên")]

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorizeDto>>> GetAuthorizes([FromQuery] PagedListParams pagedListParams)
    {
        var authorizes = await authorizeRepository.GetAuthorizeDtos(pagedListParams);

        Response.AddPaginationHeader(authorizes);

        return Ok(authorizes);
    }

}