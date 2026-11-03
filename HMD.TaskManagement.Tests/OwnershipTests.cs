using System.Net;
using HMD.TaskManagement.Application.Enums;
using HMD.TaskManagement.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HMD.TaskManagement.Tests
{
    /// <summary>Resource-level authorization: a member can only act on their own tasks, reports, notifications and profile.</summary>
    [Collection(IntegrationCollection.Name)]
    public class OwnershipTests
    {
        private readonly TmsWebApplicationFactory factory;

        public OwnershipTests(TmsWebApplicationFactory factory)
        {
            this.factory = factory;
        }

        private async Task<(TestUser Attacker, HttpClient Client, string Token, TestUser Victim, int VictimTaskId, int VictimReportId)> ArrangeAsync()
        {
            var attacker = await factory.CreateUserAsync(RoleType.Member);
            var victim = await factory.CreateUserAsync(RoleType.Member);
            var victimTaskId = await factory.CreateTaskAsync(victim.Id);
            var victimReportId = await factory.CreateReportAsync(victimTaskId);
            var client = await factory.SignedInClientAsync(attacker);
            var token = await client.GetAuthenticatedTokenAsync(attacker);
            return (attacker, client, token, victim, victimTaskId, victimReportId);
        }

        [Fact]
        public async Task Member_CannotCompleteAnotherMembersTask()
        {
            var (_, client, token, _, victimTaskId, _) = await ArrangeAsync();

            var response = await client.PostFormAsync($"/Member/AppTask/CompleteTask/{victimTaskId}", token: token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False((await factory.FindTaskAsync(victimTaskId))!.State);
        }

        [Fact]
        public async Task Member_CannotCompleteUnassignedTask()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var taskId = await factory.CreateTaskAsync(null);
            var client = await factory.SignedInClientAsync(member);
            var token = await client.GetAuthenticatedTokenAsync(member);

            var response = await client.PostFormAsync($"/Member/AppTask/CompleteTask/{taskId}", token: token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False((await factory.FindTaskAsync(taskId))!.State);
        }

        [Fact]
        public async Task Member_CannotViewAnotherMembersReports()
        {
            var (_, client, _, _, victimTaskId, victimReportId) = await ArrangeAsync();

            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/Member/AppTask/ReportList/{victimTaskId}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/Member/AppTask/UpdateReport/{victimReportId}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/Member/AppTask/CreateReport?taskId={victimTaskId}")).StatusCode);
        }

        [Fact]
        public async Task Member_CannotEditAnotherMembersReport()
        {
            var (attacker, client, token, _, victimTaskId, victimReportId) = await ArrangeAsync();
            var ownTaskId = await factory.CreateTaskAsync(attacker.Id);

            foreach (var taskId in new[] { victimTaskId, ownTaskId })
            {
                var response = await client.PostFormAsync("/Member/AppTask/UpdateReport", new Dictionary<string, string>
                {
                    ["Id"] = victimReportId.ToString(),
                    ["TaskId"] = taskId.ToString(),
                    ["Definition"] = "Hacked",
                    ["Detail"] = "Hacked",
                }, token);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            var report = await factory.FindReportAsync(victimReportId);
            Assert.Equal("Original title", report!.Defination);
            Assert.Equal("Original detail", report.Detail);
        }

        [Fact]
        public async Task Member_CannotEditAnotherMembersReport_BySpoofingUserIdField()
        {
            var (_, client, token, victim, victimTaskId, victimReportId) = await ArrangeAsync();

            var response = await client.PostFormAsync("/Member/AppTask/UpdateReport", new Dictionary<string, string>
            {
                ["Id"] = victimReportId.ToString(),
                ["TaskId"] = victimTaskId.ToString(),
                ["UserId"] = victim.Id.ToString(),
                ["Definition"] = "Hacked",
                ["Detail"] = "Hacked",
            }, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("Original title", (await factory.FindReportAsync(victimReportId))!.Defination);
        }

        [Fact]
        public async Task Member_CannotDeleteAnotherMembersReport()
        {
            var (_, client, token, _, victimTaskId, victimReportId) = await ArrangeAsync();

            var response = await client.PostFormAsync($"/Member/AppTask/DeleteReport/{victimReportId}?taskId={victimTaskId}", token: token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(await factory.FindReportAsync(victimReportId));
        }

        [Fact]
        public async Task Member_CannotCreateReportOnAnotherMembersTask()
        {
            var (_, client, token, victim, victimTaskId, _) = await ArrangeAsync();

            var response = await client.PostFormAsync("/Member/AppTask/CreateReport", new Dictionary<string, string>
            {
                ["TaskId"] = victimTaskId.ToString(),
                ["UserId"] = victim.Id.ToString(),
                ["Definition"] = "Injected",
                ["Detail"] = "Injected",
            }, token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var count = await factory.WithDbAsync(db => db.TaskReports.CountAsync(x => x.AppTaskId == victimTaskId));
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task Member_CannotMarkAnotherUsersNotificationAsRead()
        {
            var (_, client, token, victim, _, _) = await ArrangeAsync();
            var notificationId = await factory.CreateNotificationAsync(victim.Id);

            var response = await client.PostFormAsync($"/Member/Notification/Update/{notificationId}", token: token);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False((await factory.FindNotificationAsync(notificationId))!.State);
        }

        [Fact]
        public async Task Member_CannotChangeAnotherMembersProfile_BySpoofingId()
        {
            var (attacker, client, token, victim, _, _) = await ArrangeAsync();
            var victimBefore = (await factory.FindUserAsync(victim.Id))!;
            var newSurname = "Spoof_" + Guid.NewGuid().ToString("N")[..12];

            var response = await client.PostFormAsync("/Member/User/UserDetail", new Dictionary<string, string>
            {
                ["Id"] = victim.Id.ToString(),
                ["Name"] = "Spoofed",
                ["Surname"] = newSurname,
                ["Password"] = "Attacker-Chosen-1",
            }, token);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var victimAfter = (await factory.FindUserAsync(victim.Id))!;
            Assert.Equal(victimBefore.Name, victimAfter.Name);
            Assert.Equal(victimBefore.Surname, victimAfter.Surname);
            Assert.Equal(victimBefore.Password, victimAfter.Password);

            // The update was applied to the signed-in user instead.
            var attackerAfter = (await factory.FindUserAsync(attacker.Id))!;
            Assert.Equal(newSurname, attackerAfter.Surname);
        }

        [Fact]
        public async Task Member_CanCompleteOwnTask_AndEveryAdminIsNotified()
        {
            var admin1 = await factory.CreateUserAsync(RoleType.Admin);
            var admin2 = await factory.CreateUserAsync(RoleType.Admin);
            var member = await factory.CreateUserAsync(RoleType.Member);
            var title = "Own task " + Guid.NewGuid().ToString("N");
            var taskId = await factory.CreateTaskAsync(member.Id, title);
            var client = await factory.SignedInClientAsync(member);
            var token = await client.GetAuthenticatedTokenAsync(member);

            var response = await client.PostFormAsync($"/Member/AppTask/CompleteTask/{taskId}", token: token);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.True((await factory.FindTaskAsync(taskId))!.State);

            var notified = await factory.WithDbAsync(db => db.Notifications
                .Where(n => n.Description.Contains(title))
                .Select(n => n.AppUserID)
                .ToListAsync());
            Assert.Contains(admin1.Id, notified);
            Assert.Contains(admin2.Id, notified);
            Assert.DoesNotContain(member.Id, notified);
            var adminIds = await factory.WithDbAsync(db => db.Users.Where(u => u.AppRoleId == (int)RoleType.Admin).Select(u => u.Id).ToListAsync());
            Assert.All(notified, id => Assert.Contains(id, adminIds));
        }

        [Fact]
        public async Task Member_CanManageOwnReports()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var taskId = await factory.CreateTaskAsync(member.Id);
            var reportId = await factory.CreateReportAsync(taskId);
            var client = await factory.SignedInClientAsync(member);
            var token = await client.GetAuthenticatedTokenAsync(member);

            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/Member/AppTask/ReportList/{taskId}")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/Member/AppTask/UpdateReport/{reportId}")).StatusCode);

            var update = await client.PostFormAsync("/Member/AppTask/UpdateReport", new Dictionary<string, string>
            {
                ["Id"] = reportId.ToString(),
                ["TaskId"] = taskId.ToString(),
                ["Definition"] = "Updated title",
                ["Detail"] = "Updated detail",
            }, token);
            Assert.Equal(HttpStatusCode.Redirect, update.StatusCode);
            Assert.Equal("Updated title", (await factory.FindReportAsync(reportId))!.Defination);

            var create = await client.PostFormAsync("/Member/AppTask/CreateReport", new Dictionary<string, string>
            {
                ["TaskId"] = taskId.ToString(),
                ["Definition"] = "Second report",
                ["Detail"] = "Details",
            }, token);
            Assert.Equal(HttpStatusCode.Redirect, create.StatusCode);
            Assert.Equal(2, await factory.WithDbAsync(db => db.TaskReports.CountAsync(x => x.AppTaskId == taskId)));

            var delete = await client.PostFormAsync($"/Member/AppTask/DeleteReport/{reportId}?taskId={taskId}", token: token);
            Assert.Equal(HttpStatusCode.Redirect, delete.StatusCode);
            Assert.Null(await factory.FindReportAsync(reportId));
        }

        [Fact]
        public async Task Users_CanMarkOwnNotificationsAsRead()
        {
            var member = await factory.CreateUserAsync(RoleType.Member);
            var admin = await factory.CreateUserAsync(RoleType.Admin);
            var memberNotification = await factory.CreateNotificationAsync(member.Id);
            var adminNotification = await factory.CreateNotificationAsync(admin.Id);

            var memberClient = await factory.SignedInClientAsync(member);
            var memberResponse = await memberClient.PostFormAsync($"/Member/Notification/Update/{memberNotification}",
                token: await memberClient.GetAuthenticatedTokenAsync(member));
            Assert.Equal(HttpStatusCode.Redirect, memberResponse.StatusCode);
            Assert.True((await factory.FindNotificationAsync(memberNotification))!.State);

            var adminClient = await factory.SignedInClientAsync(admin);
            var adminToken = await adminClient.GetAuthenticatedTokenAsync(admin);

            // An admin cannot mark the member's notification either: it is not addressed to them.
            var foreign = await adminClient.PostFormAsync($"/Admin/Notification/Update/{memberNotification}", token: adminToken);
            Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);

            var own = await adminClient.PostFormAsync($"/Admin/Notification/Update/{adminNotification}", token: adminToken);
            Assert.Equal(HttpStatusCode.Redirect, own.StatusCode);
            Assert.True((await factory.FindNotificationAsync(adminNotification))!.State);
        }
    }
}
