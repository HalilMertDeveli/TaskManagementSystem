using System.Net;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Application.Security;
using HMD.TaskManagement.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Tests
{
    [Collection(IntegrationCollection.Name)]
    public class AuthenticationTests
    {
        private readonly TmsWebApplicationFactory factory;

        public AuthenticationTests(TmsWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Register_StoresHashedPassword()
        {
            var client = factory.CreateBrowserClient();
            var token = await client.GetAntiforgeryTokenAsync("/Account/Register");
            var username = "reg_" + Guid.NewGuid().ToString("N")[..12];
            const string password = "Register-Pass-1";

            var response = await client.PostFormAsync("/Account/Register", new Dictionary<string, string>
            {
                ["Username"] = username,
                ["Password"] = password,
                ["ConfirmPassword"] = password,
                ["Name"] = "Reg",
                ["Surname"] = "Surname_" + username,
            }, token);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains(response.RedirectPath(), new[] { "/", "/Account/Login" }); // default route renders Account/Login as "/"

            var user = await factory.WithDbAsync(db => db.Users.AsNoTracking().SingleAsync(x => x.UserName == username));
            Assert.NotEqual(password, user.Password);
            Assert.DoesNotContain(password, user.Password);
            Assert.True(PasswordSecurity.IsIdentityHash(user.Password));
            Assert.Equal(PasswordVerificationResult.Success, factory.Hasher.VerifyHashedPassword(user, user.Password, password));
            Assert.Equal((int)RoleType.Member, user.AppRoleId);
        }

        [Fact]
        public async Task Register_RejectsShortPassword()
        {
            var client = factory.CreateBrowserClient();
            var token = await client.GetAntiforgeryTokenAsync("/Account/Register");
            var username = "short_" + Guid.NewGuid().ToString("N")[..12];

            var response = await client.PostFormAsync("/Account/Register", new Dictionary<string, string>
            {
                ["Username"] = username,
                ["Password"] = "123",
                ["ConfirmPassword"] = "123",
                ["Name"] = "Reg",
                ["Surname"] = "Surname_" + username,
            }, token);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(await factory.WithDbAsync(db => db.Users.AnyAsync(x => x.UserName == username)));
        }

        [Fact]
        public async Task AdminCreatesMember_HashIsStored_TemporaryPasswordShownOnceAndWorks()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var client = await factory.SignedInClientAsync(admin);
            var token = await client.GetAntiforgeryTokenAsync("/Admin/User/Create");
            var username = "new_" + Guid.NewGuid().ToString("N")[..12];

            var response = await client.PostFormAsync("/Admin/User/Create", new Dictionary<string, string>
            {
                ["Username"] = username,
                ["Name"] = "New",
                ["Surname"] = "Member_" + username,
            }, token);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("no-store", response.Headers.CacheControl?.ToString() ?? string.Empty);
            var temporaryPassword = HttpHelpers.ExtractTemporaryPassword(await response.Content.ReadAsStringAsync());
            Assert.NotEqual("123", temporaryPassword);
            Assert.True(temporaryPassword.Length >= PasswordSecurity.MinimumLength);

            var user = await factory.WithDbAsync(db => db.Users.AsNoTracking().SingleAsync(x => x.UserName == username));
            Assert.True(PasswordSecurity.IsIdentityHash(user.Password));
            Assert.DoesNotContain(temporaryPassword, user.Password);
            Assert.Equal(PasswordVerificationResult.Success, factory.Hasher.VerifyHashedPassword(user, user.Password, temporaryPassword));

            var memberLogin = await factory.CreateBrowserClient().LoginAsync(username, temporaryPassword);
            Assert.Equal(HttpStatusCode.Redirect, memberLogin.StatusCode);
            Assert.True(memberLogin.SetsAuthCookie());
        }

        [Fact]
        public async Task Login_WithCorrectPassword_SignsIn()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);

            var response = await factory.CreateBrowserClient().LoginAsync(member.UserName, member.Password);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Member/User/UserDetail", response.RedirectPath());
            Assert.True(response.SetsAuthCookie());
        }

        [Fact]
        public async Task Login_WithWrongPassword_IsRejected()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);

            var response = await factory.CreateBrowserClient().LoginAsync(member.UserName, member.Password + "x");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(response.SetsAuthCookie());
            Assert.Contains("Kullanıcı adı veya şifre hatalı", WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()));
        }

        [Fact]
        public async Task Login_WithUnknownUser_IsRejected()
        {
            var response = await factory.CreateBrowserClient().LoginAsync("nobody_" + Guid.NewGuid().ToString("N"), "Whatever-123");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(response.SetsAuthCookie());
        }

        [Fact]
        public async Task Login_WithStoredHashAsPassword_IsRejected()
        {
            // Submitting the stored hash itself must not work (no "pass the hash").
            var member = await factory.CreateUserAsync(RoleType.Member);
            var stored = (await factory.FindUserAsync(member.Id))!.Password;

            var response = await factory.CreateBrowserClient().LoginAsync(member.UserName, stored);

            Assert.False(response.SetsAuthCookie());
        }

        [Fact]
        public async Task LegacyPlaintextPassword_LogsIn_AndIsMigratedToHash()
        {
            const string legacyPassword = "legacy-plain-123";
            var member = await factory.CreateUserAsync(RoleType.Member, legacyPassword, legacyPlaintext: true);
            Assert.Equal(legacyPassword, (await factory.FindUserAsync(member.Id))!.Password);

            // A wrong password neither signs in nor touches the stored value.
            var wrong = await factory.CreateBrowserClient().LoginAsync(member.UserName, "legacy-plain-124");
            Assert.False(wrong.SetsAuthCookie());
            Assert.Equal(legacyPassword, (await factory.FindUserAsync(member.Id))!.Password);

            var first = await factory.CreateBrowserClient().LoginAsync(member.UserName, legacyPassword);
            Assert.Equal(HttpStatusCode.Redirect, first.StatusCode);
            Assert.True(first.SetsAuthCookie());

            var migrated = (await factory.FindUserAsync(member.Id))!;
            Assert.NotEqual(legacyPassword, migrated.Password);
            Assert.True(PasswordSecurity.IsIdentityHash(migrated.Password));
            Assert.Equal(PasswordVerificationResult.Success, factory.Hasher.VerifyHashedPassword(migrated, migrated.Password, legacyPassword));

            // Still works after migration, and the hash is not rewritten again.
            var second = await factory.CreateBrowserClient().LoginAsync(member.UserName, legacyPassword);
            Assert.True(second.SetsAuthCookie());
            Assert.Equal(migrated.Password, (await factory.FindUserAsync(member.Id))!.Password);
        }

        [Fact]
        public async Task AdminResetPassword_GeneratesRandomPassword_OldPasswordStopsWorking()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var member = await factory.CreateUserAsync(RoleType.Member);
            var client = await factory.SignedInClientAsync(admin);

            var token = await client.GetAuthenticatedTokenAsync(admin);
            var first = await client.PostFormAsync($"/Admin/User/ResetPassword/{member.Id}", token: token);
            Assert.Equal(HttpStatusCode.OK, first.StatusCode);
            var tempPassword1 = HttpHelpers.ExtractTemporaryPassword(await first.Content.ReadAsStringAsync());

            var second = await client.PostFormAsync($"/Admin/User/ResetPassword/{member.Id}", token: token);
            var tempPassword2 = HttpHelpers.ExtractTemporaryPassword(await second.Content.ReadAsStringAsync());

            Assert.NotEqual(tempPassword1, tempPassword2);
            Assert.NotEqual("123", tempPassword2);

            var stored = (await factory.FindUserAsync(member.Id))!.Password;
            Assert.True(PasswordSecurity.IsIdentityHash(stored));

            Assert.False((await factory.CreateBrowserClient().LoginAsync(member.UserName, member.Password)).SetsAuthCookie());
            Assert.False((await factory.CreateBrowserClient().LoginAsync(member.UserName, tempPassword1)).SetsAuthCookie());
            Assert.True((await factory.CreateBrowserClient().LoginAsync(member.UserName, tempPassword2)).SetsAuthCookie());
        }

        [Fact]
        public async Task ProfilePage_NeverRendersThePassword()
        {
            const string password = "Profile-Pass-123";
            var member = await factory.CreateUserAsync(RoleType.Member, password);
            var client = await factory.SignedInClientAsync(member);
            var stored = (await factory.FindUserAsync(member.Id))!.Password;

            var html = await (await client.GetAsync("/Member/User/UserDetail")).Content.ReadAsStringAsync();

            Assert.DoesNotContain(password, html);
            Assert.DoesNotContain(stored, html);
            Assert.Contains("type=\"password\"", html);
        }

        [Fact]
        public async Task ProfileUpdate_HashesNewPassword_AndBlankKeepsCurrent()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var client = await factory.SignedInClientAsync(member);
            var token = await client.GetAuthenticatedTokenAsync(member);
            var surname = "Renamed_" + Guid.NewGuid().ToString("N")[..12];

            var keep = await client.PostFormAsync("/Member/User/UserDetail", new Dictionary<string, string>
            {
                ["Name"] = "Renamed",
                ["Surname"] = surname,
                ["Password"] = "",
            }, token);
            Assert.Equal(HttpStatusCode.Redirect, keep.StatusCode);
            Assert.True((await factory.CreateBrowserClient().LoginAsync(member.UserName, member.Password)).SetsAuthCookie());

            const string newPassword = "Brand-New-Pass-1";
            var change = await client.PostFormAsync("/Member/User/UserDetail", new Dictionary<string, string>
            {
                ["Name"] = "Renamed",
                ["Surname"] = surname,
                ["Password"] = newPassword,
            }, token);
            Assert.Equal(HttpStatusCode.Redirect, change.StatusCode);

            var stored = (await factory.FindUserAsync(member.Id))!;
            Assert.True(PasswordSecurity.IsIdentityHash(stored.Password));
            Assert.Equal(PasswordVerificationResult.Success, factory.Hasher.VerifyHashedPassword(stored, stored.Password, newPassword));
            Assert.False((await factory.CreateBrowserClient().LoginAsync(member.UserName, member.Password)).SetsAuthCookie());
        }

        [Fact]
        public async Task Logout_RequiresPost_AndClearsCookie()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var client = await factory.SignedInClientAsync(member);

            var get = await client.GetAsync("/Account/LogOut");
            Assert.Contains(get.StatusCode, new[] { HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound });
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/Member/AppTask/List")).StatusCode);

            var token = await client.GetAuthenticatedTokenAsync(member);
            var post = await client.PostFormAsync("/Account/LogOut", token: token);
            Assert.Equal(HttpStatusCode.Redirect, post.StatusCode);

            var after = await client.GetAsync("/Member/AppTask/List");
            Assert.Equal(HttpStatusCode.Redirect, after.StatusCode);
            Assert.StartsWith("/Account/Login", after.RedirectPath());
        }
    }
}
