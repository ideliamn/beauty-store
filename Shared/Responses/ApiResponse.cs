namespace BeautyStore.Shared.Responses
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T>(1, message, data);
        }

        public static ApiResponse<T> Error(string message)
        {
            return new ApiResponse<T>(0, message, default);
        }
    }
}
