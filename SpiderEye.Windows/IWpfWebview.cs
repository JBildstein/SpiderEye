using System;

namespace SpiderEye.Windows
{
    internal interface IWpfWebview : IWebview
    {
        event EventHandler<string> TitleChanged;
        object Control { get; }
    }
}
