using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpiderEye.Windows.Interop
{
    internal static class KeyMapper
    {
        public static Keys GetShortcut(ModifierKey modifier, Key key)
        {
            if (modifier == ModifierKey.None && key == Key.None) { return Keys.None; }

            var mappedModifier = MapModifier(modifier);
            var mappedKey = MapKey(key);

            return mappedModifier | mappedKey;
        }

        public static (ModifierKey ModifierKey, Key Key) ResolveSystemShortcut(SystemShortcut systemShortcut)
        {
            switch (systemShortcut)
            {
                case SystemShortcut.Close:
                    return (ModifierKey.Alt, Key.F4);
                case SystemShortcut.Help:
                    return (ModifierKey.None, Key.F1);
                default:
                    throw new NotSupportedException($"Unsupported system shortcut: \"{systemShortcut}\"");
            }
        }

        private static Keys MapModifier(ModifierKey modifier)
        {
            switch (modifier)
            {
                case ModifierKey.None:
                    return Keys.None;

                case ModifierKey.Shift:
                    return Keys.Shift;

                case ModifierKey.Control:
                    return Keys.Control;

                case ModifierKey.Alt:
                    return Keys.Alt;

                case ModifierKey.Primary:
                    return Keys.Control;

                case ModifierKey.Super:
                    return Keys.LWin;

                default:
                    throw new NotSupportedException($"Unsupported modifier key: \"{modifier}\"");
            }
        }

        private static Keys MapKey(Key key)
        {
            if (Keymap.TryGetValue(key, out Keys value)) { return value; }
            else { throw new NotSupportedException($"Unsupported modifier key: \"{key}\""); }
        }

        private static readonly Dictionary<Key, Keys> Keymap = new Dictionary<Key, Keys>
        {
            { Key.None, Keys.None },
            { Key.F1, Keys.F1 },
            { Key.F2, Keys.F2 },
            { Key.F3, Keys.F3 },
            { Key.F4, Keys.F4 },
            { Key.F5, Keys.F5 },
            { Key.F6, Keys.F6 },
            { Key.F7, Keys.F7 },
            { Key.F8, Keys.F8 },
            { Key.F9, Keys.F9 },
            { Key.F10, Keys.F10 },
            { Key.F11, Keys.F11 },
            { Key.F12, Keys.F12 },
            { Key.Number1, Keys.D1 },
            { Key.Number2, Keys.D2 },
            { Key.Number3, Keys.D3 },
            { Key.Number4, Keys.D4 },
            { Key.Number5, Keys.D5 },
            { Key.Number6, Keys.D6 },
            { Key.Number7, Keys.D7 },
            { Key.Number8, Keys.D8 },
            { Key.Number9, Keys.D9 },
            { Key.Number0, Keys.D0 },
            { Key.A, Keys.A },
            { Key.B, Keys.B },
            { Key.C, Keys.C },
            { Key.D, Keys.D },
            { Key.E, Keys.E },
            { Key.F, Keys.F },
            { Key.G, Keys.G },
            { Key.H, Keys.H },
            { Key.I, Keys.I },
            { Key.J, Keys.J },
            { Key.K, Keys.K },
            { Key.L, Keys.L },
            { Key.M, Keys.M },
            { Key.N, Keys.N },
            { Key.O, Keys.O },
            { Key.P, Keys.P },
            { Key.Q, Keys.Q },
            { Key.R, Keys.R },
            { Key.S, Keys.S },
            { Key.T, Keys.T },
            { Key.U, Keys.U },
            { Key.V, Keys.V },
            { Key.W, Keys.W },
            { Key.X, Keys.X },
            { Key.Y, Keys.Y },
            { Key.Z, Keys.Z },
            { Key.Insert, Keys.Insert },
            { Key.Delete, Keys.Delete },
            { Key.QuestionMark, Keys.OemQuestion },
        };
    }
}
