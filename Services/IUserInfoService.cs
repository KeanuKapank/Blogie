using Blogie.Auth.Models;
using System.Security.Claims;

namespace Blogie.UI.Services
{
    public interface IUserInfoService
    {
        int GetLogUserId();

        Task<string> GetAvatar();

        public string GetRole();
    }
}