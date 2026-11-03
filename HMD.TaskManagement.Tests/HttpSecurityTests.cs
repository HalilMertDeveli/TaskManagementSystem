using System.Net;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Domain.Entities;
using HMD.TaskManagement.Tests.Infrastructure;

namespace HMD.TaskManagement.Tests
{
    [Collection(IntegrationCollection.Name)]
    public class HttpSecurityTests
    {
        private static readonly HttpStatusCode[] NotRoutable = { HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound };

        private readonly TmsWebApplicationFactory factory;

        public HttpSecurityTests(TmsWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        private static void AssertPostFormOnly(string html, string actionPath)
        {
            Assert.DoesNotContain($"href=\"{actionPath}", html);
            var escaped = System.Text.RegularExpressions.Regex.Escape(actionPath);
            Assert.Matches($"<form(?=[^>]*action=\"{escaped}[^\"]*\")(?=[^>]*method=\"post\")[^>]*>", html);
        }

        [Fact]
        public async Task Views_RenderDestructiveActionsAsPostFormsWithToken()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var member = await factory.CreateUserAsync(RoleType.Member);
            var title = "Render " + Guid.NewGuid().ToString("N")[..10];
            var taskId = await factory.CreateTaskAsync(member.Id, title);
            var reportId = await factory.CreateReportAsync(taskId);
            var adminNotification = await factory.CreateNotificationAsync(admin.Id);
            var memberNotification = await factory.CreateNotificationAsync(member.Id);
            var memberSurname = (await factory.FindUserAsync(member.Id))!.Surname;

            var adminClient = await factory.SignedInClientAsync(admin);
            var adminPages = new Dictionary<string, string[]>
            {
                [$"/Admin/AppTask/List?s={Uri.EscapeDataString(title)}"] = new[] { $"/Admin/AppTask/Delete/{taskId}" },
                [$"/Admin/User/List?s={Uri.EscapeDataString(memberSurname)}"] = new[] { $"/Admin/User/Delete/{member.Id}", $"/Admin/User/ResetPassword/{member.Id}" },
                ["/Admin/Priority/List"] = new[] { $"/Admin/Priority/Delete/{factory.PriorityId}" },
                ["/Admin/Notification/List"] = new[] { $"/Admin/Notification/Update/{adminNotification}", "/Account/LogOut" },
            };
            foreach (var (url, actions) in adminPages)
            {
                var response = await adminClient.GetAsync(url);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var html = await response.Content.ReadAsStringAsync();
                Assert.Contains("__RequestVerificationToken", html);
                foreach (var action in actions)
                    AssertPostFormOnly(html, action);
            }

            var memberClient = await factory.SignedInClientAsync(member);
            var memberPages = new Dictionary<string, string[]>
            {
                ["/Member/AppTask/List"] = new[] { $"/Member/AppTask/CompleteTask/{taskId}" },
                [$"/Member/AppTask/ReportList/{taskId}"] = new[] { $"/Member/AppTask/DeleteReport/{reportId}" },
                ["/Member/Notification/List"] = new[] { $"/Member/Notification/Update/{memberNotification}", "/Account/LogOut" },
            };
            foreach (var (url, actions) in memberPages)
            {
                var response = await memberClient.GetAsync(url);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var html = await response.Content.ReadAsStringAsync();
                Assert.Contains("__RequestVerificationToken", html);
                foreach (var action in actions)
                    AssertPostFormOnly(html, action);
            }

            var registerHtml = await (await factory.CreateBrowserClient().GetAsync("/Account/Register")).Content.ReadAsStringAsync();
            AssertPostFormOnly(registerHtml, "/Account/Register");
        }

        [Fact]
        public async Task AdminGetRequests_CannotDeleteOrResetAnything()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var member = await factory.CreateUserAsync(RoleType.Member);
            var taskId = await factory.CreateTaskAsync(member.Id);
            var priorityId = await factory.WithDbAsync(async db =>
            {
                var p = new Priority { Defination = "Temp" };
                db.Priorities.Add(p);
                await db.SaveChangesAsync();
                return p.Id;
            });
            var notificationId = await factory.CreateNotificationAsync(admin.Id);
            var passwordBefore = (await factory.FindUserAsync(member.Id))!.Password;
            var client = await factory.SignedInClientAsync(admin);

            foreach (var url in new[]
            {
                $"/Admin/AppTask/Delete/{taskId}",
                $"/Admin/User/Delete/{member.Id}",
                $"/Admin/User/ResetPassword/{member.Id}",
                $"/Admin/Priority/Delete/{priorityId}",
                $"/Admin/Notification/Update/{notificationId}",
            })
            {
                var response = await client.GetAsync(url);
                Assert.True(NotRoutable.Contains(response.StatusCode), $"GET {url} returned {(int)response.StatusCode}");
            }

            Assert.NotNull(await factory.FindTaskAsync(taskId));
            Assert.NotNull(await factory.FindPriorityAsync(priorityId));
            Assert.False((await factory.FindNotificationAsync(notificationId))!.State);
            var userAfter = await factory.FindUserAsync(member.Id);
            Assert.NotNull(userAfter);
            Assert.Equal(passwordBefore, userAfter!.Password);
        }

        [Fact]
        public async Task MemberGetRequests_CannotCompleteOrDeleteAnything()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var taskId = await factory.CreateTaskAsync(member.Id);
            var reportId = await factory.CreateReportAsync(taskId);
            var notificationId = await factory.CreateNotificationAsync(member.Id);
            var client = await factory.SignedInClientAsync(member);

            foreach (var url in new[]
            {
                $"/Member/AppTask/CompleteTask/{taskId}",
                $"/Member/AppTask/DeleteReport/{reportId}?taskId={taskId}",
                $"/Member/Notification/Update/{notificationId}",
            })
            {
                var response = await client.GetAsync(url);
                Assert.True(NotRoutable.Contains(response.StatusCode), $"GET {url} returned {(int)response.StatusCode}");
            }

            Assert.False((await factory.FindTaskAsync(taskId))!.State);
            Assert.NotNull(await factory.FindReportAsync(reportId));
            Assert.False((await factory.FindNotificationAsync(notificationId))!.State);
        }

        [Fact]
        public async Task PostWithoutAntiforgeryToken_IsRejected()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var member = await factory.CreateUserAsync(RoleType.Member);
            var taskId = await factory.CreateTaskAsync(member.Id);

            var adminClient = await factory.SignedInClientAsync(admin);
            var deleteResponse = await adminClient.PostFormAsync($"/Admin/AppTask/Delete/{taskId}");
            Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);

            var resetResponse = await adminClient.PostFormAsync($"/Admin/User/ResetPassword/{member.Id}");
            Assert.Equal(HttpStatusCode.BadRequest, resetResponse.StatusCode);

            var memberClient = await factory.SignedInClientAsync(member);
            var completeResponse = await memberClient.PostFormAsync($"/Member/AppTask/CompleteTask/{taskId}");
            Assert.Equal(HttpStatusCode.BadRequest, completeResponse.StatusCode);

            var task = await factory.FindTaskAsync(taskId);
            Assert.NotNull(task);
            Assert.False(task!.State);
        }

        [Fact]
        public async Task PostWithInvalidAntiforgeryToken_IsRejected()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var taskId = await factory.CreateTaskAsync(null);
            var client = await factory.SignedInClientAsync(admin);

            var forged = await client.PostFormAsync($"/Admin/AppTask/Delete/{taskId}", token: "CfDJ8-not-a-real-token");
            Assert.Equal(HttpStatusCode.BadRequest, forged.StatusCode);

            // A token issued to a different session (anonymous login page) is not accepted either.
            var otherToken = await factory.CreateBrowserClient().GetAntiforgeryTokenAsync("/Account/Login");
            var foreign = await client.PostFormAsync($"/Admin/AppTask/Delete/{taskId}", token: otherToken);
            Assert.Equal(HttpStatusCode.BadRequest, foreign.StatusCode);

            Assert.NotNull(await factory.FindTaskAsync(taskId));
        }

        [Fact]
        public async Task LoginPostWithoutToken_IsRejected()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var client = factory.CreateBrowserClient();

            var response = await client.PostFormAsync("/Account/Login", new Dictionary<string, string>
            {
                ["Username"] = member.UserName,
                ["Password"] = member.Password,
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(response.SetsAuthCookie());
        }

        [Theory]
        [InlineData("/Admin/Home/Index")]
        [InlineData("/Admin/User/List")]
        [InlineData("/Admin/AppTask/List")]
        [InlineData("/Member/AppTask/List")]
        [InlineData("/Member/User/UserDetail")]
        [InlineData("/Member/Notification/List")]
        public async Task UnauthenticatedRequests_AreRedirectedToLogin(string url)
        {
            var response = await factory.CreateBrowserClient().GetAsync(url);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Account/Login", response.RedirectPath());
        }

        [Fact]
        public async Task UnauthenticatedPost_IsRedirectedToLogin_AndChangesNothing()
        {
            var taskId = await factory.CreateTaskAsync(null);
            var client = factory.CreateBrowserClient();
            var token = await client.GetAntiforgeryTokenAsync("/Account/Login");

            var response = await client.PostFormAsync($"/Admin/AppTask/Delete/{taskId}", token: token);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Account/Login", response.RedirectPath());
            Assert.NotNull(await factory.FindTaskAsync(taskId));
        }

        [Theory]
        [InlineData("/Admin/Home/Index")]
        [InlineData("/Admin/User/List")]
        [InlineData("/Admin/User/Create")]
        [InlineData("/Admin/AppTask/List")]
        [InlineData("/Admin/Priority/List")]
        [InlineData("/Admin/Notification/List")]
        public async Task Member_CannotReachAdminPages(string url)
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var client = await factory.SignedInClientAsync(member);

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Account/AccessDenied", response.RedirectPath());
        }

        [Fact]
        public async Task Member_CannotUseAdminActions_EvenWithValidToken()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var victim = await factory.CreateUserAsync(RoleType.Member);
            var taskId = await factory.CreateTaskAsync(member.Id);
            var victimPassword = (await factory.FindUserAsync(victim.Id))!.Password;
            var client = await factory.SignedInClientAsync(member);
            var token = await client.GetAuthenticatedTokenAsync(member);

            var delete = await client.PostFormAsync($"/Admin/User/Delete/{victim.Id}", token: token);
            var reset = await client.PostFormAsync($"/Admin/User/ResetPassword/{victim.Id}", token: token);
            var deleteTask = await client.PostFormAsync($"/Admin/AppTask/Delete/{taskId}", token: token);
            var create = await client.PostFormAsync("/Admin/User/Create", new Dictionary<string, string>
            {
                ["Username"] = "evil_" + Guid.NewGuid().ToString("N")[..8],
                ["Name"] = "Evil",
                ["Surname"] = "Evil_" + Guid.NewGuid().ToString("N")[..8],
            }, token);

            foreach (var response in new[] { delete, reset, deleteTask, create })
            {
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("/Account/AccessDenied", response.RedirectPath());
            }

            Assert.Equal(victimPassword, (await factory.FindUserAsync(victim.Id))!.Password);
            Assert.NotNull(await factory.FindTaskAsync(taskId));
        }

        [Fact]
        public async Task Admin_CannotUseMemberArea()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var client = await factory.SignedInClientAsync(admin);

            var response = await client.GetAsync("/Member/AppTask/List");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Account/AccessDenied", response.RedirectPath());
        }

        [Fact]
        public async Task Admin_CanDeleteTaskAndPriority_WithPostAndToken()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var taskId = await factory.CreateTaskAsync(null);
            var priorityId = await factory.WithDbAsync(async db =>
            {
                var p = new Priority { Defination = "Disposable" };
                db.Priorities.Add(p);
                await db.SaveChangesAsync();
                return p.Id;
            });
            var client = await factory.SignedInClientAsync(admin);
            var token = await client.GetAuthenticatedTokenAsync(admin);

            var deleteTask = await client.PostFormAsync($"/Admin/AppTask/Delete/{taskId}", token: token);
            Assert.Equal(HttpStatusCode.Redirect, deleteTask.StatusCode);
            Assert.Null(await factory.FindTaskAsync(taskId));

            var deletePriority = await client.PostFormAsync($"/Admin/Priority/Delete/{priorityId}", token: token);
            Assert.Equal(HttpStatusCode.Redirect, deletePriority.StatusCode);
            Assert.Null(await factory.FindPriorityAsync(priorityId));
        }

        [Fact]
        public async Task Admin_CanDeleteMember_WithPostAndToken()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var member = await factory.CreateUserAsync(RoleType.Member);
            var client = await factory.SignedInClientAsync(admin);
            var token = await client.GetAuthenticatedTokenAsync(admin);

            var response = await client.PostFormAsync($"/Admin/User/Delete/{member.Id}", token: token);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Null(await factory.FindUserAsync(member.Id));
        }

        [Fact]
        public async Task Admin_CannotResetOrDeleteAnotherAdminThroughMemberEndpoints()
        {
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var otherAdmin = await factory.CreateUserAsync(RoleType.Admin);
            var before = (await factory.FindUserAsync(otherAdmin.Id))!.Password;
            var client = await factory.SignedInClientAsync(admin);
            var token = await client.GetAuthenticatedTokenAsync(admin);

            var reset = await client.PostFormAsync($"/Admin/User/ResetPassword/{otherAdmin.Id}", token: token);
            Assert.Equal(HttpStatusCode.NotFound, reset.StatusCode);

            await client.PostFormAsync($"/Admin/User/Delete/{otherAdmin.Id}", token: token);

            var after = await factory.FindUserAsync(otherAdmin.Id);
            Assert.NotNull(after);
            Assert.Equal(before, after!.Password);
        }
    }
}
