using System;
using AppKit;

namespace SpiderEye.Mac
{
    internal class CocoaFolderSelectDialog : IFolderSelectDialog
    {
        public string? Title { get; set; }
        public string? SelectedPath { get; set; }

        public DialogResult Show()
        {
            return Show(null);
        }

        public DialogResult Show(IWindow? parent)
        {
            using var panel = NSOpenPanel.OpenPanel;
            panel.Title = Title ?? string.Empty;
            panel.CanCreateDirectories = true;
            panel.CanChooseFiles = false;
            panel.CanChooseDirectories = true;
            panel.AllowsMultipleSelection = false;

            if (!string.IsNullOrWhiteSpace(SelectedPath))
            {
                panel.DirectoryUrl = new Uri(SelectedPath);
            }

            nint result;
            if (parent == null)
            {
                result = panel.RunModal();
            }
            else
            {
                panel.BeginSheet((CocoaWindow)parent, result => NSApplication.SharedApplication.StopModalWithCode(result));
                result = NSApplication.SharedApplication.RunModalForWindow(panel);
            }

            var mappedResult = MapResult(result);
            SelectedPath = mappedResult == DialogResult.Ok ? panel.Url.Path : null;

            return mappedResult;
        }

        private static DialogResult MapResult(nint result)
        {
            return result switch
            {
                1 => DialogResult.Ok,
                0 => DialogResult.Cancel,
                _ => DialogResult.None,
            };
        }
    }
}
