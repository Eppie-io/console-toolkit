namespace Tuvi.Toolkit.Cli
{
    public class ConsoleExtension
    {
        private const string BackspaceConst = "\b \b";
        private const string Yes = "y";
        private const string No = "n";

        public static string Backspace => BackspaceConst;
        public static string NewLine => Environment.NewLine;


        public static void WriteLine(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            Console.BackgroundColor = background ?? Console.BackgroundColor;
            Console.ForegroundColor = foreground ?? Console.ForegroundColor;

            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Write(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            Console.BackgroundColor = background ?? Console.BackgroundColor;
            Console.ForegroundColor = foreground ?? Console.ForegroundColor;

            Console.Write(message);
            Console.ResetColor();
        }


        public static string? ReadValue(string query, Action<string> writer, Func<string?> reader)
        {
            writer(query);
            return reader();
        }

        public static string? ReadValue(string query)
        {
            return ReadValue(query, Console.Write, Console.ReadLine);
        }

        public static bool ReadBool(string query, Action<string> writer)
        {
            writer(query);
            var res = Console.ReadLine() ?? No;

            return res.Length > 0 && string.Compare(Yes, res[0].ToString(), true) == 0;
        }

        public static bool ReadBool(string query = "(y/n): ")
        {
            return ReadBool(query, Console.Write);
        }

        public static string? ReadPassword()
        {
            var psw = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && psw.Length > 0)
                {
                    psw = psw[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    psw += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();

            return psw;
        }

    }
}
