namespace LOAN_Web_API.Utilities
{
    public class CustomResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int StatusCode { get; private set; } // HTTP Status Code

        // Success factory method
        public static CustomResult<T> Success(T data, int statusCode = 200)
        {
            return new CustomResult<T> { IsSuccess = true, Data = data, StatusCode = statusCode };
        }

        // Failure factory method
        public static CustomResult<T> Failure(string errorMessage, int statusCode = 400)
        {
            return new CustomResult<T> { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
        }
    }
}
