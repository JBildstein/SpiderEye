using System;
using System.Collections.Generic;
using AppKit;
using ObjCRuntime;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaLabelMenuItem : CocoaMenuItem, ILabelMenuItem
    {
        public event EventHandler? Click;

        public string? Label
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value ?? string.Empty;
                if (subMenu != null) { subMenu.Title = value; }
            }
        }

        private CocoaSubMenu? subMenu;

        public CocoaLabelMenuItem(string label)
            : base()
        {
            Title = label;
            Activated += (sender, args) => Click?.Invoke(this, EventArgs.Empty);
        }

        public CocoaLabelMenuItem(string label, string action, string target)
            : this(label, action)
        {
            Target = Runtime.GetNSObject(Selector.GetHandle(target));
        }

        public CocoaLabelMenuItem(string label, string action, string? target, long tag)
            : this(label, action)
        {
            Target = target == null ? null : Runtime.GetNSObject(Selector.GetHandle(target));
            Tag = (nint)tag;
        }

        private CocoaLabelMenuItem(string label, string action)
            : base()
        {
            Title = label;
            Action = new Selector(action);
            KeyEquivalent = string.Empty;
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            NSEventModifierMask nsModifier = KeyMapper.GetModifier(modifier);
            string? mappedKey = KeyMapper.GetKey(key);
            if (mappedKey == null) { return; }

            KeyEquivalentModifierMask = nsModifier;
            KeyEquivalent = mappedKey;
        }

        public override IMenu CreateSubMenu()
        {
            return subMenu = new CocoaSubMenu(this, Label);
        }

        public CocoaMenu SetSubMenu(string label)
        {
            if (label == null) { throw new ArgumentNullException(nameof(label)); }
            if (subMenu != null) { throw new InvalidOperationException("Submenu is already created"); }

            subMenu = new CocoaSubMenu(this, label, true);
            return subMenu.NativeMenu!;
        }

        private static class KeyMapper
        {
            public static NSEventModifierMask GetModifier(ModifierKey key)
            {
                NSEventModifierMask result = 0ul;

                foreach (var flag in EnumTools.GetFlags(key))
                {
                    switch (flag)
                    {
                        case ModifierKey.None:
                            continue;

                        case ModifierKey.Shift:
                            result |= NSEventModifierMask.ShiftKeyMask;
                            break;

                        case ModifierKey.Control:
                            result |= NSEventModifierMask.ControlKeyMask;
                            break;

                        case ModifierKey.Alt:
                            result |= NSEventModifierMask.AlternateKeyMask;
                            break;

                        case ModifierKey.Super:
                        case ModifierKey.Primary:
                            result |= NSEventModifierMask.CommandKeyMask;
                            break;

                        default:
                            throw new NotSupportedException($"Unsupported modifier key: \"{flag}\"");
                    }
                }

                return result;
            }

            public static string? GetKey(Key key)
            {
                if (Keymap.TryGetValue(key, out string? value)) { return value; }
                else { throw new NotSupportedException($"Unsupported key: \"{key}\""); }
            }

            private static readonly Dictionary<Key, string?> Keymap = new()
            {
                { Key.None, null },
                { Key.F1, "\0xF704" },
                { Key.F2, "\0xF705" },
                { Key.F3, "\0xF706" },
                { Key.F4, "\0xF707" },
                { Key.F5, "\0xF708" },
                { Key.F6, "\0xF709" },
                { Key.F7, "\0xF70A" },
                { Key.F8, "\0xF70B" },
                { Key.F9, "\0xF70C" },
                { Key.F10, "\0xF70D" },
                { Key.F11, "\0xF70E" },
                { Key.F12, "\0xF70F" },
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
                { Key.Insert, "\0xF727" },
                { Key.Delete, "\0x007f" }, // Note: different than NSKey.Delete
            };
        }
    }
}
