namespace Totp_Algorithm.Utils;

public static class Base32Helper
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    /// <summary>
    /// Decodes a Base32 string into a byte array.
    /// </summary>
    public static byte[] ToBytes(string base32)
    {
        if (string.IsNullOrWhiteSpace(base32))
            throw new ArgumentException("Secret key cannot be empty.");

        base32 = base32.TrimEnd('=').ToUpper();

        // Pre-allocate buffer to prevent frequent resizing
        var output = new byte[base32.Length * 5 / 8];

        int bitBuffer = 0;
        int bitCount = 0;
        int index = 0;

        foreach (char c in base32)
        {
            int value = Alphabet.IndexOf(c);
            if (value < 0) throw new FormatException($"Invalid Base32 character: {c}");

            bitBuffer = (bitBuffer << 5) | value;
            bitCount += 5;

            if (bitCount >= 8)
            {
                bitCount -= 8;
                output[index++] = (byte)(bitBuffer >> bitCount);
            }
        }

        return output;
    }
}