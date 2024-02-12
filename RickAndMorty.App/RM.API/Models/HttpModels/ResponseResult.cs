namespace RM.API.Models.HttpModels
{
    public class ResponseResult
    {
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class ResponseResult<T> : ResponseResult
    {
        public T? Data { get; set; }
    }
}
