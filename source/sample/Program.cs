namespace Tuvi.Toolkit.Cli.Sample
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello, Console Toolkit!");

            ConsoleExtension.Write("Enter a password: ", ConsoleColor.Yellow);
            var psw = ConsoleExtension.ReadPassword();

            ConsoleExtension.WriteLine($"The password is {psw}", ConsoleColor.Cyan);

            var psw2 = ReadPassword("Enter another password: ");

            ConsoleExtension.WriteLine($"The password is {psw2}", ConsoleColor.Cyan);

            var question = ReadValue("Enter a closed question: ") ?? "Is it a closed question?";

            var answer = ConsoleExtension.ReadBool($"{question} (y/n): ");

            ConsoleExtension.WriteLine($"My answer is {(answer ? "yes" : "no")}", ConsoleColor.Cyan);
        }

        static string? ReadValue(string query)
        {
            return ConsoleExtension.ReadValue(query, (message) => ConsoleExtension.Write(message, ConsoleColor.Yellow), Console.ReadLine);
        }

        static string? ReadPassword(string query)
        {
            return ConsoleExtension.ReadValue(query, (message) => ConsoleExtension.Write(message, ConsoleColor.Yellow), () => ConsoleExtension.ReadPassword());
        }
    }
}

