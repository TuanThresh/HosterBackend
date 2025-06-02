using System.Text.Json;
using HosterBackend.Helpers;

namespace HosterBackend.Extensions;

public static class PaginationHeaderExtension
{
    public static HttpResponse AddPaginationHeader<T>(this HttpResponse httpResponse, PagedList<T> data)
    {
        var paginationHeader = new PagedListHeader<T>(data);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        httpResponse.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
        httpResponse.Headers.Append("Access-Control-Expose-Headers", "Pagination");

        return httpResponse;
    }
}