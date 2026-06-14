namespace OwnerService.DTO
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "Success";
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
