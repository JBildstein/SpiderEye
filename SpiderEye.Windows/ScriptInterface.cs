using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Windows.Internal
{
    [ComVisible(true)]
    public class ScriptInterface
    {
        internal event EventHandler<string> TitleChanged;

        internal ScriptInterface()
        {
        }

        public void ChangeTitle(string title)
        {
            TitleChanged?.Invoke(this, title);
        }
    }
}
