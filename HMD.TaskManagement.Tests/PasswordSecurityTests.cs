using HMD.TaskManagement.Application.Security;
using HMD.TaskManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Tests
{
    /// <summary>Unit tests for the hashing helpers (no database).</summary>
    public class PasswordSecurityTests
    {
        private readonly PasswordHasher<AppUser> hasher = new();

        [Fact]
        public void HasherOutput_IsRecognisedAsHash()
        {
            var hash = hasher.HashPassword(new AppUser(), "correct horse battery staple");
            Assert.True(PasswordSecurity.IsIdentityHash(hash));
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("password")]
        [InlineData("abcdefgh")]          // valid base64, but not a hasher payload
        [InlineData("AQAAAAEAACcQAAAAEA==")] // V3 header without salt/subkey
        public void PlainTextValues_AreNotTreatedAsHashes(string value)
        {
            Assert.False(PasswordSecurity.IsIdentityHash(value));
        }

        [Fact]
        public void Verify_HashedPassword_CorrectAndIncorrect()
        {
            var user = new AppUser();
            user.Password = hasher.HashPassword(user, "S3cure-password");

            Assert.Equal((true, false), PasswordSecurity.Verify(hasher, user, "S3cure-password"));
            Assert.Equal((false, false), PasswordSecurity.Verify(hasher, user, "wrong-password"));
        }

        [Fact]
        public void Verify_LegacyPlaintext_MatchesAndRequestsRehash()
        {
            var user = new AppUser { Password = "legacy-secret" };

            Assert.Equal((true, true), PasswordSecurity.Verify(hasher, user, "legacy-secret"));
            Assert.Equal((false, false), PasswordSecurity.Verify(hasher, user, "legacy-secreT"));
            Assert.Equal((false, false), PasswordSecurity.Verify(hasher, user, "legacy-secret "));
        }

        [Fact]
        public void Verify_OutdatedHash_RequestsRehash()
        {
            var v2Hasher = new PasswordHasher<AppUser>(Microsoft.Extensions.Options.Options.Create(
                new PasswordHasherOptions { CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2 }));
            var user = new AppUser();
            user.Password = v2Hasher.HashPassword(user, "old-format-pw");

            Assert.True(PasswordSecurity.IsIdentityHash(user.Password));
            Assert.Equal((true, true), PasswordSecurity.Verify(hasher, user, "old-format-pw"));
        }

        [Fact]
        public void TemporaryPasswords_AreRandomAndLongEnough()
        {
            var passwords = Enumerable.Range(0, 20).Select(_ => PasswordSecurity.GenerateTemporaryPassword()).ToList();

            Assert.All(passwords, p => Assert.True(p.Length >= PasswordSecurity.MinimumLength));
            Assert.Equal(passwords.Count, passwords.Distinct().Count());
        }
    }
}
