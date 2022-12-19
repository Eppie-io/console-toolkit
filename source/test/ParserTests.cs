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

using NUnit.Framework;

namespace Tuvi.Toolkit.Cli.CommandLine.Test
{

    // 'Tests' class is instantiated by NUnit Framework
#pragma warning disable CA1812
    [TestFixtureSource(typeof(Data.ParserData), nameof(Data.ParserData.FixtureParams))]
    [DefaultFloatingPointTolerance(1e-5)]
    class ParserTests
#pragma warning restore CA1812
    {
        public static uint DefaultUIntValue = 42;
        private Func<Action<ICommand>?, Action<ICommand>?, Action<ICommand>?, Action<ICommand>?, ICommand> Root { get; set; }
        private IParser Parser { get; init; }

        public ParserTests(IParser parser)
        {
            Parser = parser;
        }

        [SetUp]
        public void Setup()
        {
            Root = (rootAction, typeAction, commandAction, requiredAction) =>
            {
                return Parser.CreateRoot(
                    options: new List<IOption>
                    {
                        Parser.CreateOption<IEnumerable<string>>(
                            names: new List<string> {"-E", "--enumerable" },
                            allowMultipleValue: true
                        ),
                        Parser.CreateOption<string[]>(
                            names: new List<string> {"-A", "--array" },
                            allowMultipleValue: true
                        )
                    },
                    subcommands: new List<ICommand>
                    {
                        Parser.CreateCommand(
                            name: "type",
                            options: new List<IOption>
                            {
                                Parser.CreateOption<int>(
                                    names: new List<string> {"-i", "--int", "/Int" }
                                ),
                                Parser.CreateOption<uint>(
                                    names: new List<string> {"-u", "--uint", "/UInt" },
                                    getDefaultValue: () => DefaultUIntValue
                                ),
                                Parser.CreateOption<long>(
                                    names: new List<string> {"-l", "--long", "/Int64" }
                                ),
                                Parser.CreateOption<bool>(
                                    names: new List<string> {"-b", "--bool", "/Boolean" }
                                ),
                                Parser.CreateOption<float>(
                                    names: new List<string> {"-f", "--float", "/Single" }
                                ),
                                Parser.CreateOption<double>(
                                    names: new List<string> {"-d", "--double", "/Double" }
                                ),
                                Parser.CreateOption<string>(
                                    names: new List<string> {"-s", "--str", "/String" }
                                ),
                                Parser.CreateOption<Data.TestEnum>(
                                    names: new List<string> {"-e", "--enum", "/EnumOption" },
                                    getDefaultValue: () => Data.TestEnum.None
                                ),
                                Parser.CreateOption<DateTime>(
                                    names: new List<string> {"-t", "--date-time", "/DateTime" }
                                ),
                                Parser.CreateOption<Guid>(
                                    names: new List<string> {"-g", "--guid", "/Guid" }
                                ),
                            },
                            action: typeAction
                        ),
                        Parser.CreateCommand(
                            name: "command",
                            action: commandAction
                        ),
                        Parser.CreateCommand(
                            name: "command-with-required-option",
                            options: new List<IOption>
                            {
                                Parser.CreateOption<uint>(
                                    names: new List<string> {"-n", "--not-required", "/NotRequired" },
                                    valueHelpName: "uint"
                                ),
                                Parser.CreateOption<int>(
                                    names: new List<string> {"-r", "--required", "/Required" },
                                    isRequired: true,
                                    valueHelpName: "int"
                                ),
                            },
                            action: requiredAction
                        )
                    },
                    action: rootAction
                );
            };
        }

        [Test(Description = "Checking multiple values")]
        [TestCase("""-ETest --enumerable="Test" --enumerable:Test --enumerable "Test" --enumerable Test -E Test -E "Test" Test --enumerable Test Test """, 10, 0, "Test")]
        [TestCase("""-AString --array="String" --array:String --array "String" String --array String -A String -A "String" """, 0, 8, "String")]
        [TestCase("""-EText -AText -E Text -A Text  --enumerable Text "Text" Text --array Text Text "Text" """, 5, 5, "Text")]
        [TestCase("""-A"String 'Text'!" --array="String 'Text'!" "String 'Text'!" """, 0, 3, "String 'Text'!")]
        public void TestMultipleValue(string args, int countE, int countA, string value)
        {
            var rootCommand = CreateRoot(actionRoot: (cmd) =>
            {
                var optionE = cmd.FindOption<IEnumerable<string>>("-E");

                if (countE > 0)
                {
                    Assert.That(optionE?.Value, Is.Not.Null);
                    Assert.That(optionE?.Value?.Count(), Is.EqualTo(countE));

                    optionE?.Value?.ToList().ForEach((valueE) =>
                    {
                        Assert.That(valueE, Is.EqualTo(value));
                    });
                }

                var optionA = cmd.FindOption<string[]>("-A");

                if (countA > 0)
                {
                    Assert.NotNull(optionA?.Value);
                    Assert.That(optionA?.Value?.Length, Is.EqualTo(countA));

                    optionA?.Value?.ToList().ForEach((valueA) =>
                    {
                        Assert.That(valueA, Is.EqualTo(value));
                    });
                }

            });

            Parser.Bind(rootCommand);
            Parser.Invoke(args);
        }

        [Test]
        [TestCase("command", ExpectedResult = true)]
        [TestCase("", ExpectedResult = false)]
        [TestCase("unknown", ExpectedResult = false)]
        [TestCase("command -h", ExpectedResult = false)]
        [TestCase("command --help", ExpectedResult = false)]
        [TestCase("command -?", ExpectedResult = false)]
        [TestCase("command --unknown", ExpectedResult = false)]
        [TestCase("command --u", ExpectedResult = false)]
        [TestCase("command value", ExpectedResult = false)]
        [TestCase("""command "value" """, ExpectedResult = false)]
        [TestCase("type --int 42", ExpectedResult = true)]
        [TestCase("type", ExpectedResult = true)]
        [TestCase("type -i213", ExpectedResult = true)]
        [TestCase("type --uint -213", ExpectedResult = false)]
        [TestCase("type --unknown", ExpectedResult = false)]
        [TestCase("command-with-required-option", ExpectedResult = false)]
        [TestCase("command-with-required-option -r", ExpectedResult = false)]
        [TestCase("command-with-required-option -r3245", ExpectedResult = true)]
        [TestCase("command-with-required-option -n42", ExpectedResult = false)]
        [TestCase("command-with-required-option --required 1 --not-required 0", ExpectedResult = true)]
        [TestCase("command-with-required-option /Required 1 /NotRequired -1", ExpectedResult = false)]
        [TestCase("command-with-required-option /Required 1 /NotRequired 1", ExpectedResult = true)]
        [TestCase("command-with-required-option -r:-1", ExpectedResult = true)]
        public bool TestCallCommand(string args)
        {
            var isCommandCalled = false;

            Action<ICommand> action = (cmd) =>
            {
                isCommandCalled = true;
            };

            var rootCommand = CreateRoot(actionCommand: action, actionType: action, actionRequired: action);

            Parser.Bind(rootCommand);
            Parser.Invoke(args);

            return isCommandCalled;
        }

        [Test]
        [TestCaseSource(typeof(Data.ParserData), nameof(Data.ParserData.TestOptionParams))]
        public void TestOption(string args, bool isCalled, (string, bool, object?)[] options)
        {
            var isCommandCalled = false;
            var rootCommand = CreateRoot(actionType: (cmd) =>
            {
                isCommandCalled = true;
                foreach ((string name, bool exist, object? value) in options)
                {
                    var option = cmd.FindOption(name);
                    Assert.That(option, exist ? Is.Not.Null : Is.Null);

                    if (exist && value is not null)
                    {
                        Assert.That(option.Value, Is.EqualTo(value));
                    }
                }
            });

            Parser.Bind(rootCommand);
            Parser.Invoke(args);

            Assert.That(isCommandCalled, Is.EqualTo(isCalled));
        }

        [Test]
        public void TestRandomValue(
            [Random(1)] int b,
            [Random(1)] int i,
            [Random(1)] uint u,
            [Random(1)] long l,
            [Random(1)] float f,
            [Random(1)] double d,
            [Random(1)] int guidA,
            [Random(1)] short guidB,
            [Random(1)] short guidC,
            [Random(1)] long guidD)
        {
            var @bool = b > 0;

            var guid = CreateGuid(guidA, guidB, guidC, guidD);

            var args = $""" type --bool={@bool} --int={i} --uint={u} --long={l} --float={f} --double={d} --guid="{guid}" """;

            var rootCommand = CreateRoot(actionType: (cmd) =>
            {
                var optionBool = cmd.FindOption<bool>("--bool");
                Assert.That(optionBool?.Value, Is.EqualTo(@bool));

                var optionInt = cmd.FindOption<int>("--int");
                Assert.That(optionInt?.Value, Is.EqualTo(i));

                var optionUInt = cmd.FindOption<uint>("--uint");
                Assert.That(optionUInt?.Value, Is.EqualTo(u));

                var optionLong = cmd.FindOption<long>("--long");
                Assert.That(optionLong?.Value, Is.EqualTo(l));

                var optionFloat = cmd.FindOption<float>("--float");
                Assert.That(optionFloat?.Value, Is.EqualTo(f).Within(1e-5));

                var optionDouble = cmd.FindOption<double>("--double");
                Assert.That(optionDouble?.Value, Is.EqualTo(d).Within(1e-11));

                var optionGuid = cmd.FindOption<Guid>("--guid");
                Assert.That(optionGuid?.Value, Is.EqualTo(guid));
            });

            Parser.Bind(rootCommand);
            Parser.Invoke(args);
        }

        private static Guid CreateGuid(int a, short b, short c, long d)
        {
            return new Guid(a, b, c, BitConverter.GetBytes(d));
        }

        private ICommand CreateRoot(
            Action<ICommand>? actionRoot = null,
            Action<ICommand>? actionType = null,
            Action<ICommand>? actionCommand = null,
            Action<ICommand>? actionRequired = null
            )
        {
            return Root(actionRoot, actionType, actionCommand, actionRequired);
        }
    }
}
