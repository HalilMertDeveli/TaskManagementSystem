using System.Security.Cryptography;
using System.Text;
using HMD.TaskManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HMD.TaskManagement.Application.Security
{
    /// <summary>
    /// Password helpers built on ASP.NET Core Identity's <see cref="PasswordHasher{TUser}"/>.
    /// Also contains the transitional logic for accounts whose password was stored in plain text
    /// before hashing was introduced.
    /// </summary>
    public static class PasswordSecurity
    {
        public const int MinimumLength = 8;
        public const int MaximumLength = 128;

        // Unambiguous characters (no 0/O, 1/l/I) so a temporary password can be read out and typed reliably.
        private const string TemporaryPasswordAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
        private const int TemporaryPasswordLength = 16;

        /// <summary>
        /// Returns true when <paramref name="stored"/> has the binary layout produced by
        /// <see cref="PasswordHasher{TUser}"/> (Identity V2 or V3 format). Anything else is treated
        /// as a legacy plain-text value.
        /// </summary>
        public static bool IsIdentityHash(string? stored)
        {
            if (string.IsNullOrEmpty(stored) || stored.Length % 4 != 0)
                return false;

            var buffer = new byte[stored.Length];
            if (!Convert.TryFromBase64String(stored, buffer, out var length) || length == 0)
                return false;

            // V2: 0x00 | salt(16) | subkey(32)
            if (buffer[0] == 0x00)
                return length == 1 + 16 + 32;

            // V3: 0x01 | prf(4) | iterations(4) | saltLength(4) | salt | subkey
            if (buffer[0] == 0x01 && length >= 13)
            {
                var saltLength = (uint)(buffer[9] << 24 | buffer[10] << 16 | buffer[11] << 8 | buffer[12]);
                var subkeyLength = (long)length - 13 - saltLength;
                return saltLength >= 16 && subkeyLength >= 16;
            }

            return false;
        }

        /// <summary>
        /// Constant-time comparison of a legacy plain-text password with the supplied one.
        /// Both values are reduced to SHA-256 digests first so that neither the content nor the
        /// length of the stored value influences the timing.
        /// </summary>
        public static bool LegacyPlaintextEquals(string stored, string provided)
        {
            var a = SHA256.HashData(Encoding.UTF8.GetBytes(stored));
            var b = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
            return CryptographicOperations.FixedTimeEquals(a, b);
        }

        /// <summary>
        /// Verifies <paramref name="provided"/> against the user's stored password.
        /// Returns whether it matched and whether the stored value must be replaced with a fresh hash
        /// (legacy plain text, or an Identity hash with outdated parameters).
        /// </summary>
        public static (bool Success, bool NeedsRehash) Verify(IPasswordHasher<AppUser> hasher, AppUser user, string provided)
        {
            if (IsIdentityHash(user.Password))
            {
                var result = hasher.VerifyHashedPassword(user, user.Password, provided);
                return result switch
                {
                    PasswordVerificationResult.Success => (true, false),
                    PasswordVerificationResult.SuccessRehashNeeded => (true, true),
                    _ => (false, false),
                };
            }

            var legacyMatch = LegacyPlaintextEquals(user.Password ?? string.Empty, provided);
            return (legacyMatch, legacyMatch);
        }

        /// <summary>Cryptographically random temporary password for admin-initiated create/reset.</summary>
        public static string GenerateTemporaryPassword()
            => RandomNumberGenerator.GetString(TemporaryPasswordAlphabet, TemporaryPasswordLength);
    }
}
