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

namespace Tuvi.Toolkit.Cli.CommandLine
{
    public interface IParser
    {
        void Bind(ICommand root);

        static abstract ICommand CreateRoot(
            string description = "",
            List<IOption>? options = null,
            List<ICommand>? subcommands = null,
            Action<ICommand>? action = null);

        static abstract ICommand CreateCommand(
            string name,
            string description = "",
            List<IOption>? options = null,
            List<ICommand>? subcommands = null,
            Action<ICommand>? action = null);

        static abstract IOption<T> CreateOption<T>(
            List<string> names, 
            string? description = null,
            Func<T>? getDefaultValue = null,
            bool allowMultipleValue = false, 
            string? valueHelpName = null);

        void Invoke(string args);
        void Invoke(params string[] args);
    }
}
