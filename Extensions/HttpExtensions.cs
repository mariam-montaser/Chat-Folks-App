using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SocialApp.Helpers;

namespace SocialApp.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int pageSize, int totalCount, int currentPage, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, pageSize, totalCount, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("pagination", JsonSerializer.Serialize(paginationHeader,options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
            
        }
    }
}
