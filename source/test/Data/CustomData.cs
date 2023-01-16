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

using System.Globalization;
using Tuvi.Toolkit.Cli.CommandLine;

namespace Tuvi.Toolkit.Data
{
    internal struct CustomData : ICustomValue<CustomData>, IEquatable<CustomData>
    {
        public int IntValue { get; set; }
        public bool BoolValue { get; set; }

        public CustomData Parse(string data)
        {
            var args = data.Split();

            if (args.Length >= 2)
            {
                return new()
                {
                    IntValue = int.Parse(args[0], CultureInfo.DefaultThreadCurrentUICulture),
                    BoolValue = bool.Parse(args[1]),
                };
            }

            throw new InvalidOperationException();
        }

        public bool Equals(CustomData other)
        {
            return IntValue.Equals(other.IntValue) && 
                BoolValue.Equals(other.BoolValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is CustomData data && Equals(data);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
