//using LOAN_Web_API.Interfaces;
//using LOAN_Web_API.Models.Enums;
//using System.Security.Claims;

//namespace LOAN_Web_API.Services
//{
//    public class CurrentUserService : ICurrentUserService
//    {
//        private readonly IHttpContextAccessor _contextAccessor;

//        public CurrentUserService(IHttpContextAccessor contextAccessor)
//        {
//            _contextAccessor = contextAccessor;
//        }

//        public int UserId
//        {
//            get
//            {
//                var id = _contextAccessor.HttpContext?
//                    .User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//                return id != null ? int.Parse(id) : 0;
//            }
//        }

//        public Role Role
//        {
//            get
//            {
//                var role = _contextAccessor.HttpContext?
//                    .User?.FindFirst(ClaimTypes.Role)?.Value;

//                return role != null
//                    ? Enum.Parse<Role>(role)
//                    : Role.User;
//            }
//        }

//        public bool IsAdmin => Role == Role.Administrator;

//        public bool IsAuthenticated =>
//            _contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
//    }
//}
