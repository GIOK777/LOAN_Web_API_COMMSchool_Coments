using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LOAN_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        // იღებს მომხმარებლის ID-ს JWT ტოკენის Claims-დან
        protected int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idClaim == null || !int.TryParse(idClaim, out int userId))
            {
                // თუ ტოკენში ID არასწორია, ლოგიკურია Exception-ის სროლა, 
                // მაგრამ რადგან ეს მოხდება [Authorize]-ის შემდეგ, შეცდომა სავარაუდოდ 
                // კონფიგურაციაშია. მაინც უსაფრთხო დაბრუნებას ვარჩევთ.
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }
            return userId;
        }
    }
}
