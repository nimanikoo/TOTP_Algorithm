# TOTP Generator and Validator in C#

This repository provides a robust, modular, and interactive implementation of a TOTP (Time-Based One-Time Password) generator and validator. It adheres to **RFC 6238** and is compatible with Google Authenticator and Microsoft Authenticator.

The project follows **Clean Code** principles with clear separation between UI, cryptographic logic, and utilities.

---

## 🚀 Features

- **Interactive CLI Dashboard**
  - Non-blocking console UI
  - Live TOTP timer updates without flicker
- **Accurate Time Synchronization**
  - Uses Unix timestamp for exact remaining seconds
- **Modular Architecture**
  - `TotpService` for core logic
  - `Base32Helper` for decoding
- **Secure Validation**
  - Configurable time tolerance window
  - Uses `StringComparison.Ordinal`

---

## 📂 Project Structure

- `Program.cs` — Interactive console dashboard
- `Services/TotpService.cs` — TOTP generation & validation
- `Utils/Base32Helper.cs` — Base32 decoding

---

## 🛠️ How to Use

### Prerequisites

- .NET 10

### Running the Application

1. Open the project in your IDE.
2. Run the application.

Example output:

```text
========================================
 LIVE TOTP: 345487  |  Expires in: 11s
========================================
Type Code & Enter:
----------------------------------------
````

* **Generate**: Watch the header (updates every second)
* **Validate**: Enter a code and press **ENTER**

---

## 🔌 Integrating into Your Code

### Generate a Code

```csharp
using Totp_Algorithm.Services;

string secret = "JBSWY3DPEHPK3PXP";
string code = TotpService.GenerateCode(secret);
```

### Validate a Code

```csharp
using Totp_Algorithm.Services;

string userInput = "123456";
bool isValid = TotpService.ValidateCode(secret, userInput);

if (isValid)
{
    Console.WriteLine("Access Granted!");
}
```

---

## 🧠 Algorithm Overview

1. **Shared Secret** — Base32-encoded key
2. **Time Step** — Unix time ÷ 30 seconds
3. **HMAC-SHA1** — Hash time counter
4. **Dynamic Truncation** — Extract 4 bytes
5. **Modulo** — Reduce to 6 digits

---

## 🔮 Future Improvements

* [ ] QR Code generation (`otpauth://`)
* [ ] SHA256 / SHA512 support
* [ ] Replay protection

---

## 📄 License

MIT License — see [LICENSE](LICENSE)

---

## 📚 References

* [RFC 6238 – TOTP](https://datatracker.ietf.org/doc/html/rfc6238)
* [RFC 4226 – HOTP](https://datatracker.ietf.org/doc/html/rfc4226)
