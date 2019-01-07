namespace SpiderEye
{
    public interface IWebview
    {
        void LoadUrl(string url);
        void RunJs(string script);
    }
}
