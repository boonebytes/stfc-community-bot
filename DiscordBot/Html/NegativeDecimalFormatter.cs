/*
Copyright 2023 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using HandlebarsDotNet;
using HandlebarsDotNet.IO;

namespace DiscordBot.Html;

public sealed class NegativeDecimalFormatter : IFormatter, IFormatterProvider
{
    private readonly string _format;

    public NegativeDecimalFormatter(string format) => _format = format;

    public void Format<T>(T value, in EncodedTextWriter writer)
    {
        if(!(value is decimal dValue)) 
            throw new ArgumentException("supposed to be decimal");

        var output = $"{dValue.ToString(_format)}";
        if (dValue < 0) output = $"<span class=\"negative-value\">{output}</span>";
        writer.WriteSafeString(output);
    }

    public bool TryCreateFormatter(Type type, out IFormatter formatter)
    {
        if (type != typeof(decimal))
        {
            formatter = null;
            return false;
        }

        formatter = this;
        return true;
    }
}