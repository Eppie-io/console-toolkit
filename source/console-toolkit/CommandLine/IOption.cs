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
    public interface IOption
    {
        List<string> Names { get; init; }
        string? Description { get; set; }
        string? ValueHelpName { get; set; }
        bool AllowMultipleValue { get; set; }
        bool IsRequired { get; set; }
        object? Value { get; }
    }

    public interface IOption<T> : IOption
    {
        new T? Value { get; }
    }
}
