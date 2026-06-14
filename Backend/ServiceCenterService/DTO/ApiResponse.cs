namespace ServiceCenterService.DTO
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "Success"; // "Success" / "Failed" / "Error"
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
