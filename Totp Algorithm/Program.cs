using System.Security.Cryptography;

class TOTPGenerator
{
    static async Task Main()
    {
        string secret = "JBSWY3DPEHPK3PXP"; // Base32-encoded secret key
        int interval = 30; // Time step in seconds
        int digits = 6; // Number of digits in the TOTP

        Console.WriteLine("Starting TOTP generation and countdown...");
        await DisplayTOTPWithCountdownAsync(secret, interval, digits);
    }

    /// <summary>
    /// Generates a TOTP code based on the given parameters.
    /// </summary>
    /// <param name="secret">Base32-encoded secret key</param>
    /// <param name="interval">Time step in seconds</param>
    /// <param name="digits">Number of digits in the TOTP</param>
    /// <returns>The generated TOTP code</returns>
    static string GenerateTOTP(string secret, int interval, int digits)
    {
        byte[] key = Base32Decode(secret);
        long timeStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / interval;

        byte[] counter = BitConverter.GetBytes(timeStep);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counter);
        }

        using (HMACSHA1 hmac = new(key))
        {
            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[^1] & 0x0F;
            int binaryCode =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            int otp = binaryCode % (int)Math.Pow(10, digits);
            return otp.ToString(new string('0', digits));
        }
    }

    /// <summary>
    /// Validates a user-provided TOTP against the generated values within a time window.
    /// </summary>
    /// <param name="secret">Base32-encoded secret key</param>
    /// <param name="interval">Time step in seconds</param>
    /// <param name="digits">Number of digits in the TOTP</param>
    /// <param name="inputTotp">TOTP code provided by the user</param>
    /// <param name="tolerance">Number of steps to check before and after current time</param>
    /// <returns>True if the TOTP is valid; otherwise, false</returns>
    static bool ValidateTOTP(string secret, int interval, int digits, string inputTotp, int tolerance = 1)
    {
        byte[] key = Base32Decode(secret);
        long currentStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / interval;

        for (int i = -tolerance; i <= tolerance; i++)
        {
            long stepToValidate = currentStep + i;

            byte[] counter = BitConverter.GetBytes(stepToValidate);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            using HMACSHA1 hmac = new HMACSHA1(key);
            byte[] hash = hmac.ComputeHash(counter);

            int offset = hash[^1] & 0x0F;
            int binaryCode =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            int otp = binaryCode % (int)Math.Pow(10, digits);
            string generatedTotp = otp.ToString(new string('0', digits));

            if (generatedTotp == inputTotp)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Continuously generates and displays TOTP codes with a countdown timer.
    /// Timer turns red in the last 5 seconds.
    /// </summary>
    /// <param name="secret">Base32-encoded secret key</param>
    /// <param name="interval">Time step in seconds</param>
    /// <param name="digits">Number of digits in the TOTP</param>
    static async Task DisplayTOTPWithCountdownAsync(string secret, int interval, int digits)
    {
        while (true)
        {
            string totp = GenerateTOTP(secret, interval, digits);

            for (int remaining = interval; remaining > 0; remaining--)
            {
                if (remaining <= 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.Write($"\rTOTP Code: {totp} (Expires in: {remaining} seconds)");
                await Task.Delay(1000);
            }

            Console.ResetColor();
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Decodes a Base32-encoded string into a byte array.
    /// </summary>
    /// <param name="base32">The Base32-encoded string</param>
    /// <returns>The decoded byte array</returns>
    /// <exception cref="FormatException">Thrown if the input contains invalid characters</exception>
    static byte[] Base32Decode(string base32)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        base32 = base32.TrimEnd('=').ToUpper();

        int bitBuffer = 0, bitCount = 0, index = 0;
        byte[] output = new byte[base32.Length * 5 / 8];

        foreach (char c in base32)
        {
            int value = alphabet.IndexOf(c);
            if (value < 0) throw new FormatException("Invalid Base32 character.");

            bitBuffer = (bitBuffer << 5) | value;
            bitCount += 5;

            if (bitCount >= 8)
            {
                output[index++] = (byte)(bitBuffer >> (bitCount - 8));
                bitCount -= 8;
            }
        }

        return output;
    }
}
