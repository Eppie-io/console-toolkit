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

namespace Tuvi.Toolkit.Cli.CommandLine.Parser.MicrosoftCommandLine
{
    internal class Command : ICommand
    {
        public required string Name { get; init; }
        public string? Description { get; set; }
        public IReadOnlyCollection<IOption>? Options { get; set; }
        public IReadOnlyCollection<ICommand>? Subcommands { get; set; }
        public Action<ICommand>? Action { get; set; }

        public IOption? GetOption(string name)
        {
            return (from option in Options where option.Names.Contains(name) select option).FirstOrDefault();
        }

        public IOption<T>? GetOption<T>(string name)
        {
            return GetOption(name) as IOption<T>;
        }

        public IOption<T> GetRequiredOption<T>(string name)
        {
            return GetOption<T>(name) ?? throw new InvalidOperationException();
        }

        public T? GetValueOrDefualt<T>(string optionName)
        {
            var option = GetOption<T>(optionName);
            if (option is not null)
            {
                return option.Value;
            }

            return default;
        }

        public T GetRequiredValue<T>(string optionName)
        {
            return GetValueOrDefualt<T>(optionName) ?? throw new InvalidOperationException();
        }
    }

    internal class AsyncCommand : Command, ICommand, IAsyncCommand
    {
        public Func<IAsyncCommand, Task>? AsyncAction { get; set; }
    }
}
