using System.Security.Cryptography;

namespace Totp_Algorithm.Utils;

public class TotpService
{
    // Standard parameters for Google Authenticator compatibility
    private const int DefaultInterval = 30;
    private const int DefaultDigits = 6;

    /// <summary>
    /// Generates the current TOTP code based on system time.
    /// </summary>
    public static string GenerateCode(string secretKey)
    {
        var step = GetCurrentTimeStep();
        return GenerateCodeInternal(secretKey, step);
    }

    /// <summary>
    /// Validates the provided code considering a time tolerance window.
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="tolerance">Number of steps to check before/after (drift compensation).</param>
    /// <param name="secretKey"></param>
    public static bool ValidateCode(string secretKey, string inputCode, int tolerance = 1)
    {
        if (string.IsNullOrWhiteSpace(inputCode) || inputCode.Length != DefaultDigits)
            return false;

        long currentStep = GetCurrentTimeStep();

        // Check time window (backward and forward) to compensate for client/server time drift
        for (int i = -tolerance; i <= tolerance; i++)
        {
            var step = currentStep + i;
            var generatedCode = GenerateCodeInternal(secretKey, step);

            // Use StringComparison.Ordinal for safer, culture-invariant comparison
            if (string.Equals(generatedCode, inputCode, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Calculates the remaining seconds until the next code generation (useful for UI).
    /// </summary>
    public static int GetRemainingSeconds()
    {
        var currentSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return DefaultInterval - (int)(currentSeconds % DefaultInterval);
    }

    // Private helper to avoid code duplication between Generate and Validate
    private static string GenerateCodeInternal(string secretKey, long step)
    {
        byte[] key = Base32Helper.ToBytes(secretKey);

        byte[] counter = BitConverter.GetBytes(step);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counter);
        }

        using var hmac = new HMACSHA1(key);
        byte[] hash = hmac.ComputeHash(counter);

        int offset = hash[^1] & 0x0F;

        // Convert hash bytes to integer (Dynamic Truncation)
        int binaryCode =
            ((hash[offset] & 0x7F) << 24) |
            ((hash[offset + 1] & 0xFF) << 16) |
            ((hash[offset + 2] & 0xFF) << 8) |
            (hash[offset + 3] & 0xFF);

        int otp = binaryCode % (int)Math.Pow(10, DefaultDigits);

        // Pad with leading zeros (e.g., 123 becomes 000123)
        return otp.ToString(new string('0', DefaultDigits));
    }

    private static long GetCurrentTimeStep()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() / DefaultInterval;
    }
}