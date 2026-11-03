using System.Security.Claims;

namespace HMD.TaskManagement.UI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>Id of the signed-in user from the auth cookie ("UserId" claim); 0 when absent.</summary>
        public static int GetUserId(this ClaimsPrincipal user)
            => int.TryParse(user.FindFirst("UserId")?.Value, out var id) ? id : 0;
    }
}
