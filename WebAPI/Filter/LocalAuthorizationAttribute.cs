using DBManager.Pattern.Interface;
using DBManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.Collections;
using System.Security.Claims;
using System.Security.Principal;

namespace WebAPI.Filter {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LocalAuthorizationAttribute : Attribute, IAuthorizationFilter {
        private IUnitOfWork<AppDbContext> _unitOfWork;
        private IRepository<User> _userRepository;
        public void OnAuthorization(AuthorizationFilterContext context) {
            if (context.HttpContext.User.Claims.Count() <= 0) {
                context.Result = new ForbidResult();
                return;
            }
            _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork<AppDbContext>>() ?? throw new ArgumentNullException(nameof(context));
            _userRepository = _unitOfWork.GetRepository<User>() ?? throw new ArgumentNullException(nameof(_unitOfWork));
            int userId = int.Parse(context.HttpContext.User.Claims.ElementAt(0).Value);
            IEnumerable<Claim> Claims = context.HttpContext.User.Claims;
            User? user = _userRepository.Find(userId);
            if(user == null || Claims == null || user.IsLocked || Claims.ElementAt(1).Value == "True") {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
