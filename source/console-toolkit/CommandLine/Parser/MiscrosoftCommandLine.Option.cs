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

namespace Tuvi.Toolkit.Cli.CommandLine.Parser.MiscrosoftCommandLine
{
    internal interface IValueUpdater
    {
        void UpdateValue(System.CommandLine.Parsing.ParseResult result);
    }

    internal class Option<T> : System.CommandLine.Option<T>, IOption<T>, IValueUpdater
    {
        public Option(
            IReadOnlyCollection<string> names,
            string? description = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
            : base(aliases: names.ToArray(), description: description)
        {
            Names = names;
            ArgumentHelpName = valueHelpName;
            AllowMultipleArgumentsPerToken = allowMultipleValue;
            IsRequired = isRequired;
        }

        public Option(
            IReadOnlyCollection<string> names,
            Func<T> getDefaultValue,
            string? description = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
            : base(aliases: names.ToArray(), getDefaultValue: getDefaultValue, description: description)
        {
            Names = names;
            ArgumentHelpName = valueHelpName;
            AllowMultipleArgumentsPerToken = allowMultipleValue;
            IsRequired = isRequired;
        }

        // interface IOption<T>
        public IReadOnlyCollection<string> Names { get; init; }
        public string? ValueHelpName { get => ArgumentHelpName; set => ArgumentHelpName = value; }
        public bool AllowMultipleValue { get => AllowMultipleArgumentsPerToken; set => AllowMultipleArgumentsPerToken = value; }
        public T? Value { get; private set; }
        object? IOption.Value => Value;

        // interface IValueUpdater
        public void UpdateValue(System.CommandLine.Parsing.ParseResult result)
        {
            Value = result.GetValueForOption(this);
        }
    }
}
