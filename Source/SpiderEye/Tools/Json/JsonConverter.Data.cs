using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SpiderEye.Tools.Json
{
    internal partial class JsonConverter
    {
        [DebuggerStepThrough]
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
                value = *data;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Increment()
            {
                MovePosition(1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Decrement()
            {
                MovePosition(-1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void CheckCanMovePosition(int count)
            {
                int newIndex = index + count;
                if (newIndex >= length || newIndex < 0)
                {
                    throw new FormatException("JSON ended unexpectedly");
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void MovePosition(int count)
            {
                CheckCanMovePosition(count);

                index += count;
                value = data[index];
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
