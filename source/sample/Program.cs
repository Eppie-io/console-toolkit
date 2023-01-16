////////////////////////////////////////////////////////////////////////////////
//
//   Copyright 2022 Eppie(https://eppie.io)
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

using Tuvi.Toolkit.Cli.CommandLine;

namespace Tuvi.Toolkit.Cli.Sample
{
    enum EnumOption
    {
        one,
        two,
        three,
        four,
        unknown
    }

    internal static class Program
    {
        private static bool Exit { get; set; }
        private static string Hello { get; } = "Hello, Console Toolkit!";

        private static async Task Main()
        {
            Console.WriteLine(Hello);

            var parser = Create();

            while (!Exit)
            {
                var cmd = ReadValue("Command: ", Console.ForegroundColor);

                if (cmd is not null)
                {
                    await parser.InvokeAsync(cmd).ConfigureAwait(false);
                }
            }
        }

        static IAsyncParser Create()
        {
            var parser = BaseParser.Default();

            var root = parser.CreateRoot(
                description: "Command line application example.",
                options: new List<IOption>
                {
                    parser.CreateOption<IEnumerable<string>>(
                        names: "-m|--multi".Split('|').ToList(),
                        description: "Multiple option example.",
                        allowMultipleValue: true,
                        valueHelpName: "text"
                    )
                },
                subcommands: new List<ICommand>
                {
                    parser.CreateCommand(
                        name: "console",
                        description: "ConsoleExtension demo.",
                        action: (cmd) => ConsoleCommand()
                    ),
                    parser.CreateCommand(
                        name: "exit",
                        description: "Exit the application.",
                        action: (cmd) => ExitCommand()
                    ),
                    parser.CreateCommand(
                        name: "type",
                        description: "Option demo.",
                        options: new List<IOption>
                        {
                            parser.CreateOption<int>(
                                names: new List<string> {"-i", "--int", "/Int" },
                                description: "Integer option."
                            ),
                            parser.CreateOption<uint>(
                                names: new List<string> {"-u", "--uint", "/UInt" },
                                description: "Unsigned integer option.",
                                getDefaultValue: () => 111
                            ),
                            parser.CreateOption<long>(
                                names: new List<string> {"-l", "--long", "/Int64" },
                                description: "64-bit integer option."
                            ),
                            parser.CreateOption<bool>(
                                names: new List<string> {"-b", "--bool", "/Boolean" },
                                description: "Boolean option."
                            ),
                            parser.CreateOption<float>(
                                names: new List<string> {"-f", "--float", "/Single" },
                                description: "Single-precision floating-point option."
                            ),
                            parser.CreateOption<double>(
                                names: new List<string> {"-d", "--double", "/Double" },
                                description: "Double-precision floating-point option."
                            ),
                            parser.CreateOption<string>(
                                names: new List<string> {"-s", "--str", "/String" },
                                description: "String option."
                            ),
                            parser.CreateOption<EnumOption>(
                                names: new List<string> {"-e", "--enum", "/EnumOption" },
                                description: "Enum option.",
                                getDefaultValue: () => EnumOption.unknown
                            ),
                            parser.CreateOption<FileInfo>(
                                names: new List<string> {"-F", "--file", "/File" },
                                description: "FileInfo option."
                            ),
                            parser.CreateOption<DateTime>(
                                names: new List<string> {"-t", "--time", "/DateTime" },
                                description: "DateTime option."
                            ),
                        },
                        action: (cmd) =>
                        {
                            var idx = 0;
                            cmd.Options?.ToList().ForEach((option) =>
                            {
                                ConsoleExtension.WriteLine($"{++idx}. [{string.Join(", ", option.Names)}] = {option.Value} (type: {option.Value?.GetType().Name ?? "unknown"})");
                            });
                        }
                    ),
                    parser.CreateAsyncCommand(
                        name: "async",
                        description: "Async command demo.",
                        options: new List<IOption>
                        {
                            parser.CreateOption<int>(
                                names: new List<string> {"-t", "--time", "/Time" },
                                isRequired: true,
                                description: "Command execution time in milliseconds."
                            ),
                        },
                        action: async (cmd) =>
                        {
                            var value = cmd.GetRequiredValue<int>("--time");

                            Console.Write("Process");

                            var cancel = new CancellationTokenSource();
                            cancel.CancelAfter(value);

                            try
                            {
                                while(!cancel.IsCancellationRequested)
                                {
                                    await Task.Delay(1000, cancel.Token).ConfigureAwait(false);

                                    if (!cancel.IsCancellationRequested)
                                    {
                                        Console.Write(".");
                                    }
                                }
                            }
                            catch(OperationCanceledException)
                            { }
                            finally
                            {
                                Console.Write(Environment.NewLine);
                            }
                        }
                    )
                },
                action: (cmd) =>
                {
                    if (cmd.Options?.FirstOrDefault()?.Value is IEnumerable<string> items)
                    {
                        ConsoleExtension.WriteLine("Options:");
                        var idx = 0;
                        foreach (var item in items)
                        {
                            ConsoleExtension.WriteLine($"{++idx}. {item}");
                        }

                        if (idx == 0)
                        {
                            ConsoleExtension.WriteLine($"<empty>");
                        }
                    }
                }
            );

            parser.Bind(root);

            return parser;
        }


        private static void ExitCommand()
        {
            Exit = true;
        }

        private static void ConsoleCommand()
        {
            var psw = ReadPassword("Enter password: ");

            ConsoleExtension.WriteLine($"The password is {psw}", ConsoleColor.Cyan);

            var question = ReadValue("Enter a closed question: ") ?? "Is it a closed question?";

            var answer = ConsoleExtension.ReadBool($"{question} (y/n): ");

            ConsoleExtension.WriteLine($"My answer is {(answer ? "yes" : "no")}", ConsoleColor.Cyan);
        }


        private static string? ReadValue(string query, ConsoleColor foreground = ConsoleColor.Yellow)
        {
            return ConsoleExtension.ReadValue(query, (message) => ConsoleExtension.Write(message, foreground), Console.ReadLine);
        }

        private static string? ReadPassword(string query)
        {
            return ConsoleExtension.ReadValue(query, (message) => ConsoleExtension.Write(message, ConsoleColor.Yellow), () => ConsoleExtension.ReadPassword());
        }
    }
}

