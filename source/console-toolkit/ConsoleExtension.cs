////////////////////////////////////////////////////////////////////////////////
//
//   Copyright 2023 Eppie(https://eppie.io)
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
////////////////////////////////////////////////////////////////////////////////

namespace Tuvi.Toolkit.Cli
{
    public static class ConsoleExtension
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
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            writer(query);
            return reader();
        }

        public static string? ReadValue(string query)
        {
            return ReadValue(query, Console.Write, Console.ReadLine);
        }

        public static bool ReadBool(string query, Action<string> writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer(query);
            var res = Console.ReadLine() ?? No;

            return res.Length > 0 && string.Equals(Yes, res[0].ToString(), StringComparison.OrdinalIgnoreCase);
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
