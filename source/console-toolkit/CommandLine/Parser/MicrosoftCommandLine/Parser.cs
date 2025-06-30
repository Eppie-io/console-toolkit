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

namespace Tuvi.Toolkit.Cli.CommandLine.Parser.MicrosoftCommandLine
{
    internal class Parser : IParser, IAsyncParser
    {
        private System.CommandLine.Command? _root;

        private System.CommandLine.Command Root => _root ?? throw new InvalidOperationException("Root command not defined");

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
            return getDefaultValue is null
                ? new CommonOption<T>(names, description, allowMultipleValue, isRequired, valueHelpName)
                : new CommonOption<T>(names, getDefaultValue, description, allowMultipleValue, isRequired, valueHelpName);
        }

        public IOption<T> CreateCustomOption<T>(
            IReadOnlyCollection<string> names,
            Func<IEnumerable<string>, T> parseValue,
            string? description = null,
            bool allowMultipleValue = false,
            bool isRequired = false,
            string? valueHelpName = null)
        {
            return new CustomOption<T>(names, parseValue, description, allowMultipleValue, isRequired, valueHelpName);
        }

        public virtual void Bind(ICommand root)
        {
            ArgumentNullException.ThrowIfNull(root);

            _root = BindCommand(root, true);
        }

        public virtual int Invoke(string commandLine)
        {
            return Root.Parse(commandLine).Invoke();
        }

        public virtual int Invoke(params string[] args)
        {
            return Root.Parse(args).Invoke();
        }

        public virtual Task<int> InvokeAsync(string commandLine)
        {
            return Root.Parse(commandLine).InvokeAsync();
        }

        public virtual Task<int> InvokeAsync(params string[] args)
        {
            return Root.Parse(args).InvokeAsync();
        }

        private static void BindCommands(System.CommandLine.Command parent, IReadOnlyCollection<ICommand>? commands)
        {
            ArgumentNullException.ThrowIfNull(parent);

            foreach (ICommand command in commands ?? Enumerable.Empty<ICommand>())
            {
                parent.Add(BindCommand(command, false));
            }
        }

        private static System.CommandLine.Command BindCommand(ICommand command, bool isRoot = false)
        {
            ArgumentNullException.ThrowIfNull(command);

            System.CommandLine.Command cmd = isRoot
                ? new System.CommandLine.RootCommand(command.Description ?? "")
                : new System.CommandLine.Command(command.Name, command.Description);

            command.Options?.OfType<System.CommandLine.Option>().ToList().ForEach(cmd.Add);

            if (command is IAsyncCommand asyncCommand)
            {
                cmd.SetAction(async (parseResult) =>
                {
                    asyncCommand.Options?
                        .OfType<IValueUpdater>()
                        .ToList()
                        .ForEach(updater => updater.UpdateValue(parseResult));

                    if (asyncCommand.AsyncAction is not null)
                    {
                        await asyncCommand.AsyncAction.Invoke(asyncCommand).ConfigureAwait(false);
                    }
                });
            }
            else
            {
                cmd.SetAction((parseResult) =>
                {
                    command.Options?
                        .OfType<IValueUpdater>()
                        .ToList()
                        .ForEach(updater => updater.UpdateValue(parseResult));

                    command.Action?.Invoke(command);
                });
            }

            BindCommands(cmd, command.Subcommands);

            return cmd;
        }
    }
}
