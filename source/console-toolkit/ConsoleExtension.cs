// ---------------------------------------------------------------------------- //
//                                                                              //
//   Copyright 2024 Eppie (https://eppie.io)                                    //
//                                                                              //
//   Licensed under the Apache License, Version 2.0 (the "License"),            //
//   you may not use this file except in compliance with the License.           //
//   You may obtain a copy of the License at                                    //
//                                                                              //
//       http://www.apache.org/licenses/LICENSE-2.0                             //
//                                                                              //
//   Unless required by applicable law or agreed to in writing, software        //
//   distributed under the License is distributed on an "AS IS" BASIS,          //
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.   //
//   See the License for the specific language governing permissions and        //
//   limitations under the License.                                             //
//                                                                              //
// ---------------------------------------------------------------------------- //

namespace Tuvi.Toolkit.Cli
{
    public static class ConsoleExtension
    {
        private const string Yes = "y";
        private const string No = "n";

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
            string res = Console.ReadLine() ?? No;

            return res.Length > 0 && string.Equals(Yes, res[0].ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool ReadBool(string query = "(y/n): ")
        {
            return ReadBool(query, Console.Write);
        }

        public static string? ReadSecretLine(char? filler = '*')
        {
            bool ctrlC = Console.TreatControlCAsInput;

            try
            {
                string psw = string.Empty;
                ConsoleKey key;

                Console.TreatControlCAsInput = true;

                do
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                    key = keyInfo.Key;

                    if (key == ConsoleKey.Backspace && psw.Length > 0)
                    {
                        psw = psw[0..^1];

                        if (!string.IsNullOrEmpty(filler.ToString()))
                        {
                            EraseConsoleChar();
                        }
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        psw += keyInfo.KeyChar;
                        Console.Write(filler);
                    }
                    else if (char.IsControl(keyInfo.KeyChar) && keyInfo.Key == ConsoleKey.C)
                    {
                        return null;
                    }
                } while (key != ConsoleKey.Enter);

                Console.WriteLine();

                return psw;
            }
            finally
            {
                Console.TreatControlCAsInput = ctrlC;
            }
        }

        private static void EraseConsoleChar()
        {
            if (Console.CursorLeft > 0)
            {
                --Console.CursorLeft;
                Console.Write(' ');
                --Console.CursorLeft;
            }
            else if (Console.CursorTop > 0)
            {
                --Console.CursorTop;
                Console.CursorLeft = Console.WindowWidth - 1;
                Console.Write(' ');
                Console.CursorLeft = Console.WindowWidth - 1;
            }
        }

        /// <summary>
        /// Reads multiple lines from the console until the endMarker is entered on a new line (default: "EOF").
        /// </summary>
        /// <param name="query">Prompt to display before input.</param>
        /// <param name="endMarker">Line that signals the end of input. Default is "EOF".</param>
        /// <returns>All entered lines joined by Environment.NewLine, or null if no input.</returns>
        public static string? ReadMultiLine(string query, string endMarker = "EOF")
        {
            if (string.IsNullOrWhiteSpace(endMarker))
            {
                throw new ArgumentNullException(nameof(endMarker));
            }

            if (!string.IsNullOrEmpty(query))
            {
                Console.WriteLine(query);
            }

            List<string> lines = [];

            while (true)
            {
                string? line = Console.ReadLine();
                if (line == null)
                {
                    return null;
                }

                if (line == endMarker)
                {
                    break;
                }

                lines.Add(line);
            }

            return lines.Count > 0 ? string.Join(Environment.NewLine, lines) : null;
        }
    }
}
