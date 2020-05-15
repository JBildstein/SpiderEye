using System;
using System.Collections.Generic;
using System.Text;
using SpiderEye.Tools;

namespace SpiderEye.UI.Platforms.Linux.Interop
{
    internal static class KeyMapper
    {
        public static string GetShortcut(ModifierKey modifier, Key key)
        {
            if (modifier == ModifierKey.None && key == Key.None) { return null; }

            var builder = new StringBuilder();
            MapModifier(builder, modifier);
            MapKey(builder, key);

            return builder.ToString();
        }

        public static (ModifierKey ModifierKey, Key Key) ResolveSystemShortcut(SystemShortcut systemShortcut)
        {
            return systemShortcut switch
            {
                SystemShortcut.Close => (ModifierKey.Control, Key.Q),
                SystemShortcut.Help => (ModifierKey.None, Key.F1),
                _ => throw new NotSupportedException($"Unsupported system shortcut: \"{systemShortcut}\""),
            };
        }

        private static void MapModifier(StringBuilder builder, ModifierKey modifier)
        {
            foreach (var flag in EnumTools.GetFlags(modifier))
            {
                string value;
                switch (flag)
                {
                    case ModifierKey.None:
                        continue;

                    case ModifierKey.Shift:
                        value = "<Shift>";
                        break;

                    case ModifierKey.Control:
                        value = "<Control>";
                        break;

                    case ModifierKey.Alt:
                        value = "<Alt>";
                        break;

                    case ModifierKey.Super:
                    case ModifierKey.Primary:
                        value = "<Super>";
                        break;

                    default:
                        throw new NotSupportedException($"Unsupported modifier key: \"{flag}\"");
                }

                builder.Append(value);
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
            { Key.F1, "<F1>" },
            { Key.F2, "<F2>" },
            { Key.F3, "<F3>" },
            { Key.F4, "<F4>" },
            { Key.F5, "<F5>" },
            { Key.F6, "<F6>" },
            { Key.F7, "<F7>" },
            { Key.F8, "<F8>" },
            { Key.F9, "<F9>" },
            { Key.F10, "<F10>" },
            { Key.F11, "<F11>" },
            { Key.F12, "<F12>" },
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
            { Key.Insert, "<Insert>" },
            { Key.Delete, "<Delete>" },
            { Key.QuestionMark, "?" },
        };
    }
}
