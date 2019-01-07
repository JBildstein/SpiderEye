namespace SpiderEye
{
    public interface IApplication
    {
        IWindow MainWindow { get; }
        void Run();
    }
}
