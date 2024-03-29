﻿// ---------------------------------------------------------------------------- //
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

namespace Tuvi.Toolkit.Cli.CommandLine
{
    public interface ICommand
    {
        string Name { get; }
        string? Description { get; }
        IReadOnlyCollection<IOption>? Options { get; }
        IReadOnlyCollection<ICommand>? Subcommands { get; }
        Action<ICommand>? Action { get; }

        IOption? GetOption(string name);
        IOption<T>? GetOption<T>(string name);

        IOption<T> GetRequiredOption<T>(string name);
        T? GetRequiredValue<T>(string optionName);
    }

    public interface IAsyncCommand : ICommand
    {
        Func<IAsyncCommand, Task>? AsyncAction { get; }
    }
}
