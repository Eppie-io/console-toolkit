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
    internal class Command : ICommand
    {
        public required string Name { get; init; }
        public string? Description { get; set; }
        public IReadOnlyCollection<IOption>? Options { get; set; }
        public IReadOnlyCollection<ICommand>? Subcommands { get; set; }
        public Action<ICommand>? Action { get; set; }

        public IOption? FindOption(string name)
        {
            return (from option in Options where option.Names.Contains(name) select option).FirstOrDefault();
        }

        public IOption<T>? FindOption<T>(string name)
        {
            return FindOption(name) as IOption<T>;
        }
    }
}
