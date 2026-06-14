using ServiceCenterService.DTO;

namespace ServiceCenterService.Helpers
{
    public static class ResponseFactory
    {
        public static ApiResponse<T> Success<T>(string message, T data) =>
            new ApiResponse<T> { Status = "Success", Message = message, Data = data };

        public static ApiResponse<T> Failed<T>(string message) =>
            new ApiResponse<T> { Status = "Failed", Message = message, Data = default };

        public static ApiResponse<T> Error<T>(string message) =>
            new ApiResponse<T> { Status = "Error", Message = message, Data = default };
    }

}
