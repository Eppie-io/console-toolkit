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
    internal class Parser : IParser, IAsyncParser
    {
        private System.CommandLine.Command? _root;

        public virtual ICommand CreateRoot(
            string description = "",
            IReadOnlyCollection<IOption>? options = null,
            IReadOnlyCollection<ICommand>? subcommands = null,
            Action<ICommand>? action = null)
        {
            return CreateCommand("", description, options, subcommands, action);
        }

        public virtual ICommand CreateCommand(
            string name,
            string description = "",
            IReadOnlyCollection<IOption>? options = null,
            IReadOnlyCollection<ICommand>? subcommands = null,
            Action<ICommand>? action = null)
        {
            return new Command()
            {
                Name = name,
                Description = description,
                Options = options,
                Subcommands = subcommands,
                Action = action
            };
        }

        public IAsyncCommand CreateAsyncRoot(
            string description = "",
            IReadOnlyCollection<IOption>? options = null,
            IReadOnlyCollection<ICommand>? subcommands = null,
            Func<IAsyncCommand, Task>? action = null)
        {
            return CreateAsyncCommand("", description, options, subcommands, action);
        }

        public IAsyncCommand CreateAsyncCommand(
            string name,
            string description = "",
            IReadOnlyCollection<IOption>? options = null,
            IReadOnlyCollection<ICommand>? subcommands = null,
            Func<IAsyncCommand, Task>? action = null)
        {
            return new AsyncCommand()
            {
                Name = name,
                Description = description,
                Options = options,
                Subcommands = subcommands,
                AsyncAction = action
            };
        }

        public virtual IOption<T> CreateOption<T>(
            IReadOnlyCollection<string> names,
            string? description = null,
            Func<T>? getDefaultValue = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
        {
            if (getDefaultValue is not null)
            {
                return new Option<T>(names, getDefaultValue, description, allowMultipleValue, isRequired, valueHelpName);
            }

            return new Option<T>(names, description, allowMultipleValue, isRequired, valueHelpName);
        }

        public virtual void Bind(ICommand root)
        {
            _root = BindCommand(root, true);
        }

        public virtual int Invoke(string commandLine)
        {
            if (_root is null)
            {
                throw new InvalidOperationException("Root command not defined");
            }

            return System.CommandLine.CommandExtensions.Invoke(_root, commandLine);
        }

        public virtual int Invoke(params string[] args)
        {
            if (_root is null)
            {
                throw new InvalidOperationException("Root command not defined");
            }

            return System.CommandLine.CommandExtensions.Invoke(_root, args);
        }

        public virtual Task<int> InvokeAsync(string commandLine)
        {
            if (_root is null)
            {
                throw new InvalidOperationException("Root command not defined");
            }

            return System.CommandLine.CommandExtensions.InvokeAsync(_root, commandLine);
        }

        public virtual Task<int> InvokeAsync(params string[] args)
        {
            if (_root is null)
            {
                throw new InvalidOperationException("Root command not defined");
            }

            return System.CommandLine.CommandExtensions.InvokeAsync(_root, args);
        }

        private static void BindCommands(System.CommandLine.Command parent, IReadOnlyCollection<ICommand>? commands)
        {
            foreach (var command in commands ?? Enumerable.Empty<ICommand>())
            {
                parent.AddCommand(BindCommand(command, false));
            }
        }

        private static System.CommandLine.Command BindCommand(ICommand command, bool isRoot = false)
        {
            System.CommandLine.Command cmd = (isRoot)
                ? new System.CommandLine.RootCommand(command.Description ?? "")
                : new System.CommandLine.Command(command.Name, command.Description);

            command.Options?.OfType<System.CommandLine.Option>().ToList().ForEach(cmd.AddOption);

            if (command is IAsyncCommand asyncCommand)
            {
                System.CommandLine.Handler.SetHandler(cmd, async (context) =>
                {
                    asyncCommand.Options?.OfType<IValueUpdater>().ToList().ForEach((updater) => { updater.UpdateValue(context.ParseResult); });

                    if (asyncCommand.AsyncAction is not null)
                    {
                        await asyncCommand.AsyncAction.Invoke(asyncCommand).ConfigureAwait(false);
                    }
                });
            }
            else
            {
                System.CommandLine.Handler.SetHandler(cmd, (context) =>
                {
                    command.Options?.OfType<IValueUpdater>().ToList().ForEach((updater) => { updater.UpdateValue(context.ParseResult); });
                    command.Action?.Invoke(command);
                });
            }

            BindCommands(cmd, command.Subcommands);

            return cmd;
        }
    }
}
