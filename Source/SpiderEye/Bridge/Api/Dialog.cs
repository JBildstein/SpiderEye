using System;
using SpiderEye.Bridge.Models;
using SpiderEye.UI;

namespace SpiderEye.Bridge.Api
{
    internal class Dialog
    {
        private readonly IWindow parent;
        private readonly IUiFactory windowFactory;

        public Dialog(IWindow parent, IUiFactory windowFactory)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));
        }

        public DialogResult ShowMessageBox(MessageBoxConfigModel config)
        {
            var msgBox = windowFactory.CreateMessageBox();
            msgBox.Title = config.Title;
            msgBox.Message = config.Message;
            msgBox.Buttons = config.Buttons;

            return msgBox.Show(parent);
        }

        public FileResultModel ShowSaveFileDialog(SaveFileDialogConfigModel config)
        {
            var dialog = windowFactory.CreateSaveFileDialog();
            dialog.Title = config.Title;
            dialog.InitialDirectory = config.InitialDirectory;
            dialog.FileName = config.FileName;
            dialog.OverwritePrompt = config.OverwritePrompt;

            if (config.FileFilters != null)
            {
                foreach (var filter in config.FileFilters)
                {
                    dialog.FileFilters.Add(filter.ToFilter());
                }
            }

            var result = dialog.Show(parent);
            return new FileResultModel
            {
                DialogResult = result,
                File = dialog.FileName,
                Files = new string[] { dialog.FileName },
            };
        }

        public FileResultModel ShowOpenFileDialog(OpenFileDialogConfigModel config)
        {
            var dialog = windowFactory.CreateOpenFileDialog();
            dialog.Title = config.Title;
            dialog.InitialDirectory = config.InitialDirectory;
            dialog.FileName = config.FileName;
            dialog.Multiselect = config.Multiselect;

            if (config.FileFilters != null)
            {
                foreach (var filter in config.FileFilters)
                {
                    dialog.FileFilters.Add(filter.ToFilter());
                }
            }

            var result = dialog.Show(parent);
            return new FileResultModel
            {
                DialogResult = result,
                File = dialog.FileName,
                Files = dialog.SelectedFiles,
            };
        }
    }
}
