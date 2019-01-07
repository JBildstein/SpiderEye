namespace SpiderEye
{
    public interface IWindow
    {
        IWebview Webview { get; }
        string Title { get; set; }

        void Show();
    }
}
