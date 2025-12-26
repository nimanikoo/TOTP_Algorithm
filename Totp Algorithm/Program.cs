using System.Text;
using Totp_Algorithm.Utils;

namespace Totp_Algorithm;

class Program
{
    const string Secret = "JBSWY3DPEHPK3PXP";

    static void Main()
    {
        Console.CursorVisible = false; // Hide blinking cursor for cleaner UI
        var inputBuffer = new StringBuilder();

        while (true)
        {
            // 1. Calculate and Draw UI
            var remaining = TotpService.GetRemainingSeconds();
            var code = TotpService.GenerateCode(Secret);

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("========================================");
            Console.WriteLine($" LIVE TOTP: {code}  |  Expires in: {remaining:D2}s   "); 
            Console.WriteLine("========================================");
            Console.WriteLine("Type Code & Enter: " + inputBuffer.ToString() + "      "); // Extra spaces to clear old chars
            Console.WriteLine("----------------------------------------");
            
            // 2. Check for User Input (Non-blocking)
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true); // Read key without printing it automatically

                if (key.Key == ConsoleKey.Enter)
                {
                    string input = inputBuffer.ToString();
                    inputBuffer.Clear(); // Clear buffer for next try

                    Console.WriteLine($"\n> Validating '{input}'...");
                    
                    bool isValid = TotpService.ValidateCode(Secret, input);
                    if (isValid)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  RESULT: VALID [✓]");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  RESULT: INVALID [X]");
                    }
                    Console.ResetColor();
                    Console.WriteLine("----------------------------------------");
                    // Pause briefly so user can see the result before loop redraws too much
                    Thread.Sleep(1500); 
                    Console.Clear(); // Clean slate after validation
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (inputBuffer.Length > 0)
                        inputBuffer.Length--;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    break; // Exit app
                }
                else
                {
                    // Add character to buffer
                    inputBuffer.Append(key.KeyChar);
                }
            }

            // 3. Small delay to prevent high CPU usage, but fast enough for UI
            Thread.Sleep(100); 
        }
    }
}