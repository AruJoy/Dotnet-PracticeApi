using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using PracticeApi.Application.Common.Attributes;
using PracticeApi.Application.Common.Response;

namespace PracticeApi.Infrastructure.Swagger
{
    /// <summary>
    /// [ApiProducesResponse] → Swagger에 실제 ProducesResponseType으로 변환
    /// </summary>
    public class ApiProducesResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo
                .GetCustomAttributes(typeof(ApiProducesResponseAttribute), false)
                .Cast<ApiProducesResponseAttribute>()
                .ToList();

            foreach (var attr in attributes)
            {
                var wrappedType = typeof(ApiResponse<>).MakeGenericType(attr.ResponseType);
                context.ApiDescription.SupportedResponseTypes.Add(new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiResponseType
                {
                    StatusCode = attr.StatusCode,
                    Type = wrappedType,
                    ApiResponseFormats = new List<Microsoft.AspNetCore.Mvc.ApiExplorer.ApiResponseFormat>
                    {
                        new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiResponseFormat
                        {
                            MediaType = "application/json"
                        }
                    }
                });
            }
        }
    }
}