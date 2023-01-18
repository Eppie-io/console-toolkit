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

namespace Tuvi.Toolkit.Data
{
    internal record CustomData
    {
        public required int IntValue { get; init; }
        public required bool BoolValue { get; init; }

        protected static CustomData Parse(string data)
        {
            try
            {
                var args = data.Split();

                if (args.Length == 2)
                {
                    return new()
                    {
                        IntValue = int.Parse(args[0], CultureInfo.CurrentCulture),
                        BoolValue = bool.Parse(args[1]),
                    };
                }
            }
            catch (FormatException) { }
            catch (OverflowException) { }

            throw new CustomDataParseException(nameof(data));
        }

        public static CustomData Parse(IEnumerable<string> data)
        {
            return Parse(data.FirstOrDefault() ?? string.Empty);
        }

        public static IEnumerable<CustomData> ParseList(IEnumerable<string> data)
        {
            return data.Select(Parse);
        }
    }

    public class CustomDataParseException : Exception
    {
        private static string DefaultMessage { get; } = "Data can't be parsed";

        public CustomDataParseException(string paramName)
            : base(DefaultMessage, new ArgumentException(DefaultMessage, paramName))
        {
        }

        protected CustomDataParseException()
            : base(DefaultMessage)
        {
        }

        protected CustomDataParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
