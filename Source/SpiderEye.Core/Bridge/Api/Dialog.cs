using System;
using SpiderEye.Bridge.Models;

namespace SpiderEye.Bridge.Api
{
    internal class Dialog
    {
        private readonly Window parent;

        public Dialog(Window parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public DialogResult ShowMessageBox(MessageBoxConfigModel config)
        {
            return Application.Invoke(() => MessageBox.Show(parent, config.Message, config.Title, config.Buttons));
        }

        public FileResultModel ShowSaveFileDialog(SaveFileDialogConfigModel config)
        {
            var dialog = new SaveFileDialog
            {
                Title = config.Title,
                InitialDirectory = config.InitialDirectory,
                FileName = config.FileName,
                OverwritePrompt = config.OverwritePrompt,
            };

            if (config.FileFilters != null)
            {
                foreach (var filter in config.FileFilters)
                {
                    dialog.FileFilters.Add(filter.ToFilter());
                }
            }

            var result = Application.Invoke(() => dialog.Show(parent));
            return new FileResultModel
            {
                DialogResult = result,
                File = dialog.FileName,
                Files = new string[] { dialog.FileName },
            };
        }

        public FileResultModel ShowOpenFileDialog(OpenFileDialogConfigModel config)
        {
            var dialog = new OpenFileDialog
            {
                Title = config.Title,
                InitialDirectory = config.InitialDirectory,
                FileName = config.FileName,
                Multiselect = config.Multiselect,
            };

            if (config.FileFilters != null)
            {
                foreach (var filter in config.FileFilters)
                {
                    dialog.FileFilters.Add(filter.ToFilter());
                }
            }

            var result = Application.Invoke(() => dialog.Show(parent));
            return new FileResultModel
            {
                DialogResult = result,
                File = dialog.FileName,
                Files = dialog.SelectedFiles,
            };
        }
    }
}
