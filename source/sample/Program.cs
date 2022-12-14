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
    using MicrosoftParser = Tuvi.Toolkit.Cli.CommandLine.Parser.MiscrosoftCommandLine.Parser;
    enum EnumOption
    {
        one,
        two,
        three,
        four,
        unknown
    }

    static class Program
    {
        static bool Exit { get; set; } = false;
        static void Main()
        {
            try
            {
                Console.WriteLine("Hello, Console Toolkit!");

                var parser = Create();

                while (!Exit)
                {
                    var cmd = ReadValue("Command: ", Console.ForegroundColor);

                    if (cmd is not null)
                    {
                        parser.Invoke(cmd);
                    }
                }
            }
            catch(Exception ex)
            {
                ConsoleExtension.WriteLine(ex.ToString(), ConsoleColor.Red);
                Environment.ExitCode = -1;
            }
        }

        static IParser Create()
        {
            var parser = new MicrosoftParser();

            var root = MicrosoftParser.CreateRoot(
                description: "Command line application example.",
                options: new List<IOption>
                {
                    MicrosoftParser.CreateOption<IEnumerable<string>>(
                        names: "-m|--multi".Split('|').ToList(),
                        description: "Multiple option example.",
                        allowMultipleValue: true,
                        valueHelpName: "text"
                    )
                },
                subcommands: new List<ICommand>
                {
                    MicrosoftParser.CreateCommand(
                        name: "console",
                        description: "ConsoleExtension demo.",
                        action: (cmd) => ConsoleCommand()
                    ),
                    MicrosoftParser.CreateCommand(
                        name: "exit",
                        description: "Exit the application.",
                        action: (cmd) => ExitCommand()
                    ),
                    MicrosoftParser.CreateCommand(
                        name: "type",
                        description: "Option demo.",
                        options: new List<IOption>
                        {
                            MicrosoftParser.CreateOption<int>(
                                names: new List<string> {"-i", "--int", "/Int" },
                                description: "Integer option."
                            ),
                            MicrosoftParser.CreateOption<uint>(
                                names: new List<string> {"-u", "--uint", "/UInt" },
                                description: "Unsigned integer option.",
                                getDefaultValue: () => 111
                            ),
                            MicrosoftParser.CreateOption<long>(
                                names: new List<string> {"-l", "--long", "/Int64" },
                                description: "64-bit integer option."
                            ),
                            MicrosoftParser.CreateOption<bool>(
                                names: new List<string> {"-b", "--bool", "/Boolean" },
                                description: "Boolean option."
                            ),
                            MicrosoftParser.CreateOption<float>(
                                names: new List<string> {"-f", "--float", "/Single" },
                                description: "Single-precision floating-point option."
                            ),
                            MicrosoftParser.CreateOption<double>(
                                names: new List<string> {"-d", "--double", "/Double" },
                                description: "Double-precision floating-point option."
                            ),
                            MicrosoftParser.CreateOption<string>(
                                names: new List<string> {"-s", "--str", "/String" },
                                description: "String option."
                            ),
                            MicrosoftParser.CreateOption<EnumOption>(
                                names: new List<string> {"-e", "--enum", "/EnumOption" },
                                description: "Enum option.",
                                getDefaultValue: () => EnumOption.unknown
                            ),
                            MicrosoftParser.CreateOption<FileInfo>(
                                names: new List<string> {"-F", "--file", "/File" },
                                description: "FileInfo option."
                            ),
                            MicrosoftParser.CreateOption<DateTime>(
                                names: new List<string> {"-t", "--time", "/DateTime" },
                                description: "DateTime option."
                            ),
                        },
                        action: (cmd) =>
                        {
                            var idx = 0;
                            cmd.Options?.ForEach((option) =>
                            {
                                ConsoleExtension.WriteLine($"{++idx}. [{string.Join(", ", option.Names)}] = {option.Value} (type: {option.Value?.GetType().Name ?? "unknown"})");
                            });
                        }
                    )
                },
                action: (cmd) =>
                {
                    if (cmd.Options?.Count > 0 && cmd.Options[0].Value is IEnumerable<string> items)
                    {
                        ConsoleExtension.WriteLine("Options:");
                        var idx = 0;
                        foreach (var item in items)
                        {
                            ConsoleExtension.WriteLine($"{++idx}. {item}");
                        }
                    }
                }
            );

            parser.Bind(root);

            return parser;
        }


        static void ExitCommand()
        {
            Program.Exit = true;
        }

        static void ConsoleCommand()
        {
            var psw = ReadPassword("Enter password: ");

            ConsoleExtension.WriteLine($"The password is {psw}", ConsoleColor.Cyan);

            var question = ReadValue("Enter a closed question: ") ?? "Is it a closed question?";

            var answer = ConsoleExtension.ReadBool($"{question} (y/n): ");

            ConsoleExtension.WriteLine($"My answer is {(answer ? "yes" : "no")}", ConsoleColor.Cyan);
        }


        static string? ReadValue(string query, ConsoleColor foreground = ConsoleColor.Yellow)
        {
            return ConsoleExtension.ReadValue(query, (message) => ConsoleExtension.Write(message, foreground), Console.ReadLine);
        }

        static string? ReadPassword(string query)
        {
            return ConsoleExtension.ReadValue(query, (message) => ConsoleExtension.Write(message, ConsoleColor.Yellow), () => ConsoleExtension.ReadPassword());
        }
    }
}

