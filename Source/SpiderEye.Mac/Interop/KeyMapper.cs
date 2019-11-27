using System;
using System.Collections.Generic;
using SpiderEye.Tools;

namespace SpiderEye.Mac.Interop
{
    internal static class KeyMapper
    {
        public static NSEventModifierFlags GetModifier(ModifierKey key)
        {
            NSEventModifierFlags result = NSEventModifierFlags.None;

            foreach (var flag in EnumTools.GetFlags(key))
            {
                switch (flag)
                {
                    case ModifierKey.None:
                        continue;

                    case ModifierKey.Shift:
                        result |= NSEventModifierFlags.Shift;
                        break;

                    case ModifierKey.Control:
                        result |= NSEventModifierFlags.Control;
                        break;

                    case ModifierKey.Alt:
                        result |= NSEventModifierFlags.Option;
                        break;

                    case ModifierKey.Super:
                    case ModifierKey.Primary:
                        result |= NSEventModifierFlags.Command;
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported modifier key: \"{flag}\"");
                }
            }

            return result;
        }

        public static string GetKey(Key key)
        {
            if (Keymap.TryGetValue(key, out string value)) { return value; }
            else { throw new NotSupportedException($"Unsupported key: \"{key}\""); }
        }

        private static readonly Dictionary<Key, string> Keymap = new Dictionary<Key, string>
        {
            { Key.None, null },
            { Key.F1, NSKey.F1 },
            { Key.F2, NSKey.F2 },
            { Key.F3, NSKey.F3 },
            { Key.F4, NSKey.F4 },
            { Key.F5, NSKey.F5 },
            { Key.F6, NSKey.F6 },
            { Key.F7, NSKey.F7 },
            { Key.F8, NSKey.F8 },
            { Key.F9, NSKey.F9 },
            { Key.F10, NSKey.F10 },
            { Key.F11, NSKey.F11 },
            { Key.F12, NSKey.F12 },
            { Key.Number1, "1" },
            { Key.Number2, "2" },
            { Key.Number3, "3" },
            { Key.Number4, "4" },
            { Key.Number5, "5" },
            { Key.Number6, "6" },
            { Key.Number7, "7" },
            { Key.Number8, "8" },
            { Key.Number9, "9" },
            { Key.Number0, "0" },
            { Key.A, "a" },
            { Key.B, "b" },
            { Key.C, "c" },
            { Key.D, "d" },
            { Key.E, "e" },
            { Key.F, "f" },
            { Key.G, "g" },
            { Key.H, "h" },
            { Key.I, "i" },
            { Key.J, "j" },
            { Key.K, "k" },
            { Key.L, "l" },
            { Key.M, "m" },
            { Key.N, "n" },
            { Key.O, "o" },
            { Key.P, "p" },
            { Key.Q, "q" },
            { Key.R, "r" },
            { Key.S, "s" },
            { Key.T, "t" },
            { Key.U, "u" },
            { Key.V, "v" },
            { Key.W, "w" },
            { Key.X, "x" },
            { Key.Y, "y" },
            { Key.Z, "z" },
            { Key.Insert, NSKey.Insert },
            { Key.Delete, "\0x007f" }, // Note: different than NSKey.Delete
        };
    }
}
