using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SpiderEye.Windows.Interop
{
    internal static class KeyMapper
    {
        public static Shortcut GetShortcut(ModifierKey modifier, Key key)
        {
            if (modifier == ModifierKey.None && key == Key.None) { return Shortcut.None; }

            var builder = new StringBuilder();
            MapModifier(builder, modifier);
            MapKey(builder, key);

            string name = builder.ToString();
            if (Enum.TryParse(name, out Shortcut shortcut)) { return shortcut; }
            else { throw new NotSupportedException($"Unsupported shortcut: \"{name}\""); }
        }

        private static void MapModifier(StringBuilder builder, ModifierKey modifier)
        {
            switch (modifier)
            {
                case ModifierKey.None:
                    return;

                case ModifierKey.Shift:
                    builder.Append("Shift");
                    break;

                case ModifierKey.Control:
                    builder.Append("Ctrl");
                    break;

                case ModifierKey.Alt:
                    builder.Append("Alt");
                    break;

                case ModifierKey.Control | ModifierKey.Shift:
                    builder.Append("CtrlShift");
                    break;

                case ModifierKey.Super:
                case ModifierKey.Primary:
                default:
                    throw new NotSupportedException($"Unsupported modifier key: \"{modifier}\"");
            }
        }

        private static void MapKey(StringBuilder builder, Key key)
        {
            if (Keymap.TryGetValue(key, out string value)) { builder.Append(value); }
            else { throw new NotSupportedException($"Unsupported modifier key: \"{key}\""); }
        }

        private static readonly Dictionary<Key, string> Keymap = new Dictionary<Key, string>
        {
            { Key.None, string.Empty },
            { Key.F1, "F1" },
            { Key.F2, "F2" },
            { Key.F3, "F3" },
            { Key.F4, "F4" },
            { Key.F5, "F5" },
            { Key.F6, "F6" },
            { Key.F7, "F7" },
            { Key.F8, "F8" },
            { Key.F9, "F9" },
            { Key.F10, "F10" },
            { Key.F11, "F11" },
            { Key.F12, "F12" },
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
            { Key.A, "A" },
            { Key.B, "B" },
            { Key.C, "C" },
            { Key.D, "D" },
            { Key.E, "E" },
            { Key.F, "F" },
            { Key.G, "G" },
            { Key.H, "H" },
            { Key.I, "I" },
            { Key.J, "J" },
            { Key.K, "K" },
            { Key.L, "L" },
            { Key.M, "M" },
            { Key.N, "N" },
            { Key.O, "O" },
            { Key.P, "P" },
            { Key.Q, "Q" },
            { Key.R, "R" },
            { Key.S, "S" },
            { Key.T, "T" },
            { Key.U, "U" },
            { Key.V, "V" },
            { Key.W, "W" },
            { Key.X, "X" },
            { Key.Y, "Y" },
            { Key.Z, "Z" },
            { Key.Insert, "Ins" },
            { Key.Delete, "Del" },
        };
    }
}
