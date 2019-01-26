using System;
using SpiderEye.UI;

namespace SpiderEye.Bridge.Api
{
    internal class Dialog
    {
        private readonly IWindow parent;
        private readonly IWindowFactory windowFactory;

        public Dialog(IWindow parent, IWindowFactory windowFactory)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));
        }

        // TODO: add methods to show various dialogs
    }
}
