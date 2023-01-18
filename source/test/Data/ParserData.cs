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

using NUnit.Framework;
using System.Collections;
using Tuvi.Toolkit.Data;

namespace Tuvi.Toolkit.Cli.CommandLine.Test.Data
{
    internal enum TestEnum
    {
        Red,
        Black,
        White,
        None
    }

    internal class ParserData
    {
        public static IEnumerable FixtureParams
        {
            get
            {
                yield return new TestFixtureData(BaseParser.MicrosoftParser());
            }
        }

        public static IEnumerable TestOptionParams
        {
            get
            {
                yield return new TestCaseData($"type --uint {int.MinValue}", false, Array.Empty<(string, bool, object?)>());
                yield return new TestCaseData($"type --int {long.MinValue}", false, Array.Empty<(string, bool, object?)>());

                yield return new TestCaseData($"type --enum {nameof(ConsoleColor.Green)}", false, Array.Empty<(string, bool, object?)>());
                yield return new TestCaseData($"type --enum {nameof(TestEnum.Red)}", true, new (string, bool, object?)[] { ("-e", true, TestEnum.Red) });

                yield return new TestCaseData($"""type --str "{StringWithDoubleQuoteMark}" """, false, Array.Empty<(string, bool, object?)>());
                yield return new TestCaseData($"""type --str "{StringWithSpace}" """, true, new (string, bool, object?)[] { ("/String", true, StringWithSpace) });
                yield return new TestCaseData($"type /String {StringWithoutSpace}", true, new (string, bool, object?)[] { ("-s", true, StringWithoutSpace) });

                yield return new TestCaseData($"type --bool", true, new (string, bool, object?)[] { ("--bool", true, true) });
                yield return new TestCaseData($"type -b:{bool.FalseString}", true, new (string, bool, object?)[] { ("/Boolean", true, false) });

                yield return new TestCaseData($"""type /DateTime "{Today}" """, true, new (string, bool, object?)[] { ("/DateTime", true, Today) });
                yield return new TestCaseData($"""type --date-time "{Date}" """, true, new (string, bool, object?)[] { ("-t", true, Date) });

                yield return new TestCaseData($"""type --guid "{EmptyGuid}" """, true, new (string, bool, object?)[] { ("/Guid", true, EmptyGuid) });
                yield return new TestCaseData($"""type -g "{NewGuid}" """, true, new (string, bool, object?)[] { ("/Guid", true, NewGuid) });

                yield return new TestCaseData($"type -i{int.MinValue} --long:{long.MaxValue}", true, new (string, bool, object?)[]
                {
                    ("-i", true, int.MinValue),
                    ("--int", true, int.MinValue),
                    ("/Int", true, int.MinValue),
                    ("/Int64", true, long.MaxValue),
                    ("--uint", true, ParserTests.DefaultUIntValue),
                    ("--bool", true, false),
                    ("--enum", true, TestEnum.None),
                    ("--date-time", true, default(DateTime)),
                    ("-g", true, null),
                    ("-s", true, null),
                    ("--str", true, null),
                    ("/String", true, null),
                    ("-U", false, null),
                    ("--unknown", false, null),
                    ("/Unknown", false, null),
                });

                yield return new TestCaseData($"""type --custom "{int.MinValue} {bool.TrueString}" """, true, new (string, bool, object?)[]
                {
                    ("/Custom", true, new CustomData(){IntValue = int.MinValue, BoolValue = true}),
                });
            }
        }

        private static readonly DateTime Date = new(2022, 12, 12, 11, 11, 11);
        private static readonly DateTime Today = DateTime.Today;
        private static readonly Guid EmptyGuid = Guid.Empty;
        private static readonly Guid NewGuid = TestContext.CurrentContext.Random.NextGuid();

        private static string StringWithSpace { get; } = "This is text with space";
        private static string StringWithoutSpace { get; } = "Thisisoneword";
        private static string StringWithDoubleQuoteMark { get; } = """string \"with double quote\" doesn't work""";
    }
}
