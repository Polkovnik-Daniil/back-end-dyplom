using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using System.Security.Claims;

namespace WebAPI.Filter {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LocalAuthorizationAttribute : Attribute, IAuthorizationFilter {
        public void OnAuthorization(AuthorizationFilterContext context) {
            IEnumerable<Claim> Claims = context.HttpContext.User.Claims;
            if (context.HttpContext.User.Claims.Count() > 0 && Claims != null && Claims.ElementAt(1).Value == "False") {
                IUnitOfWork<AppDbContext> _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(context));
                IRepository<User> _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
                int userId = int.Parse(context.HttpContext.User.Claims.ElementAt(0).Value);
                User? user = _userRepository.Find(userId);
                if(user != null && !user.IsLocked) {
                    return;
                }
            }
            context.Result = new ForbidResult();
            return;
        }
    }
}
