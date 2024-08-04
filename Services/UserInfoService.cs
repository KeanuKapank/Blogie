using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using Blogie.Auth.Models;
using Blogie.Auth.Repository;

namespace Blogie.UI.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthRepository _repo;
        public UserInfoService(IHttpContextAccessor httpContextAccessor, IAuthRepository repo)
        {
            _httpContextAccessor = httpContextAccessor;
            _repo = repo;
        }

        public int GetLogUserId()
        {
            var val = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(val);
        }
        
        public string GetRole()
        {
            var val = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return val.ToString();
        }



        public async Task<string> GetAvatar()
        {
            User user = await _repo.GetLogindUser(GetLogUserId());
            string imageBase64 = Convert.ToBase64String(user.Avatar);
            return imageBase64;
        }
    }

}
