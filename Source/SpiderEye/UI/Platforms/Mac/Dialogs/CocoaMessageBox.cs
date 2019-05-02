using System;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Dialogs
{
    internal class CocoaMessageBox : IMessageBox
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageBoxButtons Buttons { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow parent)
        {
            var window = parent as CocoaWindow;
            if (parent != null && window == null)
            {
                throw new ArgumentException("Invalid window type.", nameof(parent));
            }

            using (var alert = NSDialog.CreateAlert())
            {
                ObjC.Call(alert.Handle, "setShowsHelp:", IntPtr.Zero);
                ObjC.Call(alert.Handle, "setMessageText:", NSString.Create(Title));
                ObjC.Call(alert.Handle, "setInformativeText:", NSString.Create(Message));
                AddButtons(alert.Handle, Buttons);

                return (DialogResult)alert.Run(window);
            }
        }

        private void AddButtons(IntPtr alert, MessageBoxButtons buttons)
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

        private void AddButton(IntPtr alert, string title, DialogResult result)
        {
            IntPtr button = ObjC.Call(alert, "addButtonWithTitle:", NSString.Create(title));
            ObjC.Call(button, "setTag:", new IntPtr((int)result));
        }
    }
}
