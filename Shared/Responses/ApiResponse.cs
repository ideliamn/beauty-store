namespace BeautyStore.Shared.Responses
{
    public class ApiResponse<T>
    {
        public int HttpStatus { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public ApiResponse(int httpStatus, int code, string message, T data, Dictionary<string, List<string>>? errors =  null)
        {
            HttpStatus = httpStatus;
            Code = code;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> Success(int httpStatus, T data, string message = "Success")
        {
            return new ApiResponse<T>(httpStatus, 1, message, data);
        }

        public static ApiResponse<T> Error(int httpStatus, string message, Dictionary<string, List<string>>? errors = null)
        {
            return new ApiResponse<T>(httpStatus, 0, message, default, errors);
        }
    }
}
