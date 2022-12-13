////////////////////////////////////////////////////////////////////////////////
///
///   Copyright 2022 Eppie(https://eppie.io)
///
///   Licensed under the Apache License, Version 2.0 (the "License");
///   you may not use this file except in compliance with the License.
///   You may obtain a copy of the License at
///
///       http://www.apache.org/licenses/LICENSE-2.0
///
///   Unless required by applicable law or agreed to in writing, software
///   distributed under the License is distributed on an "AS IS" BASIS,
///   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///   See the License for the specific language governing permissions and
///   limitations under the License.
///
////////////////////////////////////////////////////////////////////////////////

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

