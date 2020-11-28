using System;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaMessageBox : IMessageBox
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public MessageBoxButtons Buttons { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow? parent)
        {
            var window = NativeCast.To<CocoaWindow>(parent);
            using var alert = NSDialog.CreateAlert();
            ObjC.Call(alert.Handle, "setShowsHelp:", IntPtr.Zero);
            ObjC.Call(alert.Handle, "setAlertStyle:", new UIntPtr((uint)NSAlertStyle.Informational));
            ObjC.Call(alert.Handle, "setMessageText:", NSString.Create(Title ?? string.Empty));
            ObjC.Call(alert.Handle, "setInformativeText:", NSString.Create(Message ?? string.Empty));
            AddButtons(alert.Handle, Buttons);

            return (DialogResult)alert.Run(window);
        }

        private static void AddButtons(IntPtr alert, MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OkCancel:
                    AddButton(alert, "Ok", DialogResult.Ok);
                    AddButton(alert, "Cancel", DialogResult.Cancel);
                    break;

                case MessageBoxButtons.YesNo:
                    AddButton(alert, "Yes", DialogResult.Yes);
                    AddButton(alert, "No", DialogResult.No);
                    break;

                case MessageBoxButtons.Ok:
                default:
                    AddButton(alert, "Ok", DialogResult.Ok);
                    break;
            }
        }

        private static void AddButton(IntPtr alert, string title, DialogResult result)
        {
            IntPtr button = ObjC.Call(alert, "addButtonWithTitle:", NSString.Create(title));
            ObjC.Call(button, "setTag:", new IntPtr((int)result));
        }
    }
}
