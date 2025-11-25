using Serilog;
using System.Net;

namespace LOAN_Web_API.Middleware
{
    // Middleware პასუხისმგებელია მთლიან API-ში დაუგეგმავი Exception-ების დაჭერაზე.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // გადავდივართ Pipeline-ის შემდეგ კომპონენტზე (Controllers, Services)
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // თუ Exception მოხდა:
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 1. ლოგირება (ყველაზე მნიშვნელოვანი ნაწილი)
            // SeriLog ავტომატურად დაიჭერს ამ ლოგს.
            Log.Error(exception, "An unhandled exception occurred during the request to: {Path}", context.Request.Path);

            // 2. HTTP პასუხის კონფიგურაცია
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500

            // 3. უსაფრთხო, ზოგადი პასუხის დაბრუნება (უსაფრთხოების დაცვა)
            var responseBody = new
            {
                // ეს არის ერთადერთი ინფორმაცია, რომელსაც მომხმარებელი მიიღებს PRODUCTION-ში
                error = "An unexpected error occurred. Please try again later.",
                // Dev-ში შეგვეძლო დამატებითი დეტალების დამატება, მაგრამ Production-ისთვის საკმარისია
            };

            return context.Response.WriteAsJsonAsync(responseBody);
        }
    }
}
