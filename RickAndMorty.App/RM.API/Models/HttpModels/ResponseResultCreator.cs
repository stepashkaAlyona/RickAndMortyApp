namespace RM.API.Models.HttpModels
{
    public static class ResponseResultCreator
    {
        public static ResponseResult Error(string errorMessage)
        {
            return new ResponseResult { IsSuccess = false, ErrorMessage = errorMessage };
        }

        public static ResponseResult Success()
        {
            return new ResponseResult { IsSuccess = true };
        }

        public static ResponseResult Success<T>(T data)
        {
            return new ResponseResult<T> { IsSuccess = true, Data = data };
        }
    }
}