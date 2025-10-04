namespace PracticeApi.Application.Common.Response
{
    // apiResponse 의 제네릭 설정
    // <T> => data 를 반환하지만, Type을 특정하지는 않음, 응답 본문에 포함될 데이터의 타입을 지정
    public class ApiResponse<T>
    {
        // 전역 response 의 구조

        // success : api 성공여부
        public bool Success { get; set; } = true;
        // message 기본적으로 OK 사용
        public string Message { get; set; } = "OK";
        // 응답에 따라 data를 포함히서 반환
        // 비어있을 수 있음
        public T? Data { get; set; }
        // - Success: true
        // - Message: "OK" 또는 전달된 message
        // - Data: 제네릭 타입 T의 인스턴스
        public static ApiResponse<T> Ok(T data, string? message = null)
            => new() { Success = true, Message = message ?? "OK", Data = data };
        // - Success: false
        // - Message: 전달된 오류 메시지
        // - Data: null (실패 시 데이터 없음)
        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message, Data = default };
    }
}