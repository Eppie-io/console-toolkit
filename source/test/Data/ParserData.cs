using NUnit.Framework;
using System.Collections;

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
                yield return new TestFixtureData(Default.MicrosoftParser());
            }
        }

        public static IEnumerable TestOptionParams
        {
            get
            {
                yield return new TestCaseData($"type --uint {int.MinValue}", false, new (string, bool, object?)[] { });
                yield return new TestCaseData($"type --int {long.MinValue}", false, new (string, bool, object?)[] { });

                yield return new TestCaseData($"type --enum {nameof(ConsoleColor.Green)}", false, new (string, bool, object?)[] { });
                yield return new TestCaseData($"type --enum {nameof(TestEnum.Red)}", true, new (string, bool, object?)[] { ("-e", true, TestEnum.Red) });

                yield return new TestCaseData($"""type --str "{StringWithDoubleQuoteMark}" """, false, new (string, bool, object?)[] { });
                yield return new TestCaseData($"""type --str "{StringWithSpace}" """, true, new (string, bool, object?)[] { ("/String", true, StringWithSpace) });
                yield return new TestCaseData($"type /String {StringWithoutSpace}", true, new (string, bool, object?)[] { ("-s", true, StringWithoutSpace) });

                yield return new TestCaseData($"type --bool", true, new (string, bool, object?)[] { ("--bool", true, true) });
                yield return new TestCaseData($"type -b:{Boolean.FalseString}", true, new (string, bool, object?)[] { ("/Boolean", true, false) });

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
            }
        }

        private static DateTime Date = new DateTime(2022, 12, 12, 11, 11, 11);
        private static DateTime Today = DateTime.Today;
        private static Guid EmptyGuid = Guid.Empty;
        private static Guid NewGuid = TestContext.CurrentContext.Random.NextGuid();

        private static string StringWithSpace = "This is text with space";
        private static string StringWithoutSpace = "Thisisoneword";
        private static string StringWithDoubleQuoteMark = """string \"with double quote\" doesn't work""";
    }
}
