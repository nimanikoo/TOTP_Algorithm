# TOTP Generator and Validator in C#

This repository provides a simple yet robust implementation of a TOTP (Time-Based One-Time Password) generator and validator. It leverages the HMAC-SHA1 algorithm, supports Base32 secret encoding, and adheres to the TOTP standards specified in [RFC 6238](https://datatracker.ietf.org/doc/html/rfc6238). The implementation ensures compatibility with popular authenticator apps like Google Authenticator and Microsoft Authenticator.

## Features

- **TOTP Generation**:
  - Generates one-time passwords using a shared secret and current time.
  - Configurable time intervals (default: 30 seconds) and digit lengths (default: 6 digits).
- **TOTP Validation**:
  - Validates user-provided TOTP codes with built-in time tolerance to account for minor clock drift.
- **Real-Time Countdown**:
  - Displays a countdown timer indicating the time remaining until the TOTP code expires.
  - Visual warning (red text) during the last 5 seconds.
- **Asynchronous Design**:
  - Fully asynchronous implementation for improved responsiveness and performance.

---

## Algorithm Overview

TOTP combines a shared secret key and the current time to generate a one-time password. Here’s how it works:

1. **Shared Secret**: A Base32-encoded secret key is pre-shared between the client (e.g., user’s authenticator app) and the server.
2. **Time Step**: The current Unix timestamp is divided by a fixed interval (default: 30 seconds) to calculate a time step.
3. **HMAC Hashing**:
   - The time step is encoded as an 8-byte counter (big-endian).
   - The counter is hashed using HMAC-SHA1 with the shared secret as the key.
4. **Dynamic Truncation**:
   - A subset of the HMAC result is extracted using an offset from the hash.
   - The extracted portion is converted to a 31-bit integer.
5. **Modulo Operation**:
   - The integer is reduced to the desired number of digits using modulo (e.g., modulo 10^6 for 6-digit codes).
6. **Validation**:
   - The server generates TOTP codes for the current and previous time steps and compares them with the user-provided code.

This algorithm ensures that TOTP codes are time-sensitive and expire after the configured interval.

---

## How to Use

### Prerequisites

1. Install the .NET 9.0 SDK from [Microsoft's website](https://dotnet.microsoft.com/).
2. Clone or download this repository.

### Running the Code

1. Open the project in your preferred IDE (e.g., Rider or Visual Studio or Visual Studio Code).
2. Update the `secret` variable in the `Main` method with your Base32-encoded shared key:
   ```csharp
   string secret = "JBSWY3DPEHPK3PXP"; // Replace with your Base32-encoded key
   ```
3. Compile and run the application.

The program will:
- Generate a new TOTP code every 30 seconds.
- Display the current TOTP code along with a countdown timer.
- Change the timer's color to red during the last 5 seconds.

### Example Output

```plaintext
TOTP Code: 123456 (Expires in: 29 seconds)
TOTP Code: 123456 (Expires in: 28 seconds)
...
TOTP Code: 123456 (Expires in: 5 seconds) [Red Text]
```

### Validating TOTP Codes

To validate a user-provided TOTP code, use the `ValidateTOTP` method:

```csharp
bool isValid = ValidateTOTP(secret, userInput, 30, 6);
if (isValid)
{
    Console.WriteLine("Code is valid!");
}
else
{
    Console.WriteLine("Invalid code.");
}
```

---

## Improvements

1. **Enhanced Security**:
   - Replace HMAC-SHA1 with HMAC-SHA256 or HMAC-SHA512 for stronger hashing.
   - Implement rate-limiting to prevent brute force attacks.
2. **User Management**:
   - Store secrets securely (e.g., encrypted in a database).
   - Support multiple users with separate TOTP keys.
3. **Web Integration**:
   - Create a web API using .NET to generate and validate TOTP codes.
   - Add a user-friendly frontend for managing TOTP settings.
4. **Customizable Configurations**:
   - Allow users to customize intervals, digits, and hashing algorithms.

---

## Dependencies

- .NET 9.0 SDK

---

## License

This project is open-source and available under the [MIT License](LICENSE). Feel free to use and modify it as needed.

---

## References

- [RFC 6238: TOTP: Time-Based One-Time Password Algorithm](https://datatracker.ietf.org/doc/html/rfc6238)
- [HMAC: Keyed-Hashing for Message Authentication](https://datatracker.ietf.org/doc/html/rfc2104)

## Contributions

Contributions are welcome! If you find a bug or have a feature request, please open an issue or submit a pull request. Let's build something great together!



