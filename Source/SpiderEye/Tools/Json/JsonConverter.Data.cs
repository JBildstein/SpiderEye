using System;
using System.Runtime.CompilerServices;

namespace SpiderEye.Tools.Json
{
    internal static partial class JsonConverter
    {
        private unsafe sealed class JsonData
        {
            public int Index
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return index; }
            }

            public char* Pointer
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return data + index; }
            }

            public char Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return value; }
            }

            private readonly char* data;
            private readonly int length;

            private int index;
            private char value;

            public JsonData(char* data, int length)
            {
                this.data = data;
                this.length = length;
            }

            public void Increment()
            {
                Increment(1);
            }

            public void Increment(int count)
            {
                if (index + count < length)
                {
                    index += count;
                    value = data[index];
                }
                else { throw new FormatException("JSON ended unexpectedly"); }
            }

            public string GetDisplayValue()
            {
                switch (value)
                {
                    case '\0':
                        return "\\0";

                    case '\r':
                        return "\\r";

                    case '\n':
                        return "\\n";

                    case '\t':
                        return "\\t";
                }

                if (value < 32) { return $"0x{Convert.ToString(value, 16)}"; }

                return value.ToString();
            }
        }
    }
}
