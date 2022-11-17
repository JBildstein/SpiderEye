using AppKit;

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
            using var alert = new NSAlert();
            alert.ShowsHelp = false;
            alert.AlertStyle = NSAlertStyle.Informational;
            alert.MessageText = Title ?? string.Empty;
            alert.InformativeText = Message ?? string.Empty;

            var buttonDetails = Buttons switch
            {
                MessageBoxButtons.OkCancel => new[] { ("Ok", DialogResult.Ok), ("Cancel", DialogResult.Cancel) },
                MessageBoxButtons.YesNo => new[] { ("Yes", DialogResult.Yes), ("No", DialogResult.No) },
                _ => new[] { ("Ok", DialogResult.Ok) },
            };

            foreach (var (title, dialogResult) in buttonDetails)
            {
                using var button = alert.AddButton(title);
                button.Tag = (int)dialogResult;
            }

            nint result;
            if (parent == null)
            {
                result = alert.RunModal();
            }
            else
            {
                alert.BeginSheet((CocoaWindow)parent, response => NSApplication.SharedApplication.StopModalWithCode((nint)response));
                result = NSApplication.SharedApplication.RunModalForWindow(alert.Window);
            }

            return (DialogResult)result;
        }
    }
}
