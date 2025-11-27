namespace LOAN_Web_API.Utilities
{

    // CustomResult<T> საშუალებას გვაძლევს, დავაბრუნოთ წარმატების ან წარუმატებლობის შედეგი მეთოდიდან,
    // Exception-ის სროლის გარეშე. მეთოდი ყოველთვის წარმატებით სრულდება,
    // მაგრამ შედეგი გეუბნებათ, იყო თუ არა ბიზნეს ოპერაცია წარმატებული.
    public class CustomResult<T>
    {
        public bool IsSuccess { get; private set; } //იყო თუ არა ოპერაცია წარმატებული (თუ true, შეგვიძლია Data-ს წაკითხვა).
        public T? Data { get; private set; } // წარმატების შემთხვევაში, აქ ინახება შედეგი (მაგ., LoanResponseDto, UserToken).
        public string? ErrorMessage { get; private set; } // ადამიანისთვის გასაგები აღწერა (მაგ., "User is blocked").
        public int StatusCode { get; private set; } // HTTP Status Code

        // Success factory method წარმატება
        public static CustomResult<T> Success(T data, int statusCode = 200)
        {
            return new CustomResult<T> { IsSuccess = true, Data = data, StatusCode = statusCode };
        }

        // Failure factory method წარუმატებლობა
        public static CustomResult<T> Failure(string errorMessage, int statusCode = 400)
        {
            return new CustomResult<T> { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
        }
    }
}
