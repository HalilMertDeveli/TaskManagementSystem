using System.Net;
using System.Text.RegularExpressions;

namespace HMD.TaskManagement.Tests.Infrastructure
{
    public static partial class HttpHelpers
    {
        public const string AuthCookieName = "PersonalManagementLCookie";

        [GeneratedRegex("<input[^>]*name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
        private static partial Regex TokenRegex();

        [GeneratedRegex("<code id=\"temporary-password\"[^>]*>([^<]+)</code>")]
        private static partial Regex TemporaryPasswordRegex();

        /// <summary>Loads a page and returns the antiforgery token embedded in one of its POST forms.</summary>
        public static async Task<string> GetAntiforgeryTokenAsync(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            var match = TokenRegex().Match(html);
            Assert.True(match.Success, $"No antiforgery token found on {url}");
            return WebUtility.HtmlDecode(match.Groups[1].Value);
        }

        public static Task<HttpResponseMessage> PostFormAsync(this HttpClient client, string url, IDictionary<string, string>? fields = null, string? token = null)
        {
            var values = new Dictionary<string, string>(fields ?? new Dictionary<string, string>());
            if (token != null)
                values["__RequestVerificationToken"] = token;
            return client.PostAsync(url, new FormUrlEncodedContent(values));
        }

        public static async Task<HttpResponseMessage> LoginAsync(this HttpClient client, string username, string password)
        {
            var token = await client.GetAntiforgeryTokenAsync("/Account/Login");
            return await client.PostFormAsync("/Account/Login", new Dictionary<string, string>
            {
                ["Username"] = username,
                ["Password"] = password,
            }, token);
        }

        public static async Task<HttpClient> SignedInClientAsync(this TmsWebApplicationFactory factory, TestUser user)
        {
            var client = factory.CreateBrowserClient();
            var response = await client.LoginAsync(user.UserName, user.Password);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            return client;
        }

        /// <summary>Token for an authenticated session, read from the logout form in the area layout.</summary>
        public static Task<string> GetAuthenticatedTokenAsync(this HttpClient client, TestUser user)
            => client.GetAntiforgeryTokenAsync(user.Role == HMD.TaskManagement.Application.Enums.RoleType.Admin
                ? "/Admin/Notification/List"
                : "/Member/Notification/List");

        public static bool SetsAuthCookie(this HttpResponseMessage response)
            => response.Headers.TryGetValues("Set-Cookie", out var cookies)
               && cookies.Any(c => c.StartsWith(AuthCookieName + "=", StringComparison.Ordinal) && !c.StartsWith(AuthCookieName + "=;", StringComparison.Ordinal));

        public static string? RedirectPath(this HttpResponseMessage response)
            => response.Headers.Location is { } location
                ? (location.IsAbsoluteUri ? location.PathAndQuery : location.OriginalString)
                : null;

        public static string ExtractTemporaryPassword(string html)
        {
            var match = TemporaryPasswordRegex().Match(html);
            Assert.True(match.Success, "Temporary password was not shown");
            return WebUtility.HtmlDecode(match.Groups[1].Value);
        }
    }
}
