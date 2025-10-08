using Microsoft.AspNetCore.Mvc;
using PracticeApi.Application.Common.Response;
using System;

namespace PracticeApi.Application.Common.Attributes
{
    /// <summary>
    /// Swagger/OpenAPI 문서에서 ApiResponse<T> 래핑 구조를 자동 등록하기 위한 커스텀 Attribute
    /// </summary>

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ApiProducesResponseAttribute : Attribute
    {
        public Type ResponseType { get; }
        public int StatusCode { get; }
        // [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        // attribute 형식에 필요한 지역변수
        public ApiProducesResponseAttribute(Type responseType, int statusCode = 200)
        {
            ResponseType = responseType;
            StatusCode = statusCode;
        }

        /// <summary>
        /// Swagger 문서 생성을 위한 ASP.NET Core 표준 Attribute 변환기
        /// </summary>
        public ProducesResponseTypeAttribute ToProducesResponseType()
        {
            // ApiResponse에서 각각의 응댭형식을 generic 으로 생성
            var wrappedType = typeof(ApiResponse<>).MakeGenericType(ResponseType);
            // ms MVC 의 attribute 객체 봔환
            return new ProducesResponseTypeAttribute(wrappedType, StatusCode);
        }

    }
}