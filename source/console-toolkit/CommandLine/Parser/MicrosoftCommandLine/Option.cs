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

using System.CommandLine;
using System.CommandLine.Parsing;

namespace Tuvi.Toolkit.Cli.CommandLine.Parser.MicrosoftCommandLine
{
    internal interface IValueUpdater
    {
        void UpdateValue(ParseResult result);
    }

    internal class CommonOption<T> : System.CommandLine.Option<T>, IOption<T>, IValueUpdater
    {
        public CommonOption(
            IReadOnlyCollection<string> names,
            string? description = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
            : base(name: names.First(), aliases: names.ToArray())
        {
            Names = names;
            ValueHelpName = valueHelpName;
            Description = description;
            AllowMultipleArgumentsPerToken = allowMultipleValue;
            Required = isRequired;
        }

        public CommonOption(
            IReadOnlyCollection<string> names,
            Func<ArgumentResult, T> getDefaultValue,
            string? description = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
            : base(name: names.First(), aliases: names.ToArray())
        {
            Names = names;
            ValueHelpName = valueHelpName;
            Description = description;
            DefaultValueFactory = getDefaultValue;
            AllowMultipleArgumentsPerToken = allowMultipleValue;
            Required = isRequired;
        }

        // interface IOption<T>
        public IReadOnlyCollection<string> Names { get; init; }
        public string? ValueHelpName { get; set; }
        public bool AllowMultipleValue { get => AllowMultipleArgumentsPerToken; set => AllowMultipleArgumentsPerToken = value; }
        public T? Value { get; private set; }
        public bool IsRequired => Required;

        object? IOption.Value => Value;

        // interface IValueUpdater
        public void UpdateValue(ParseResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            Value = result.GetValue(this);
        }
    }

    internal class CustomOption<T> : CommonOption<T>
    {
        public CustomOption(
            IReadOnlyCollection<string> names,
            Func<IEnumerable<string>, T> parseValue,
            string? description = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
            : base(names, description, allowMultipleValue, isRequired, valueHelpName)
        {
            Arity = allowMultipleValue ? ArgumentArity.OneOrMore : ArgumentArity.ExactlyOne;
            CustomParser = (arg) => ParseArgument(arg, parseValue);
        }

        private static T ParseArgument(ArgumentResult result, Func<IEnumerable<string>, T> parser)
        {
            ArgumentNullException.ThrowIfNull(result);
            ArgumentNullException.ThrowIfNull(parser);

            IEnumerable<string> data = result.Tokens.Select((token) => token.Value);
            return parser(data);
        }
    }
}
