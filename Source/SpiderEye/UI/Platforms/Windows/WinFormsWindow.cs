using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SpiderEye.Bridge;
using SpiderEye.Content;
using SpiderEye.Tools;
using SpiderEye.UI.Windows.Interop;

namespace SpiderEye.UI.Windows
{
    internal class WinFormsWindow : Form, IWindow
    {
        public event PageLoadEventHandler PageLoaded
        {
            add { webview.PageLoaded += value; }
            remove { webview.PageLoaded -= value; }
        }

        event CancelableEventHandler IWindow.Closing
        {
            add { ClosingBackingEvent += value; }
            remove { ClosingBackingEvent -= value; }
        }

        event EventHandler IWindow.Closed
        {
            add { ClosedBackingEvent += value; }
            remove { ClosedBackingEvent -= value; }
        }

        private event CancelableEventHandler ClosingBackingEvent;
        private event EventHandler ClosedBackingEvent;

        public IWebviewBridge Bridge
        {
            get { return bridge; }
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public bool CanResize
        {
            get { return FormBorderStyle == FormBorderStyle.Sizable; }
            set
            {
                FormBorderStyle = value ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
                MaximizeBox = value;
            }
        }

        private readonly ContentServer server;
        private readonly WebviewBridge bridge;
        private readonly IWinFormsWebview webview;

        public WinFormsWindow(WindowConfiguration config, IUiFactory windowFactory)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (windowFactory == null) { throw new ArgumentNullException(nameof(windowFactory)); }

            bridge = new WebviewBridge();
            var contentProvider = new EmbeddedFileProvider(config.ContentAssembly, config.ContentFolder);
            if (!config.ForceWindowsLegacyWebview && IsEdgeAvailable())
            {
                webview = new WinFormsWebview(config, contentProvider, bridge);
            }
            else
            {
                string hostAddress;
                if (string.IsNullOrWhiteSpace(config.ExternalHost))
                {
                    server = new ContentServer(contentProvider);
                    server.Start();
                    hostAddress = server.HostAddress;
                }
                else { hostAddress = config.ExternalHost; }

                webview = new WinFormsLegacyWebview(config, hostAddress, bridge);
            }

            webview.Control.Location = new Point(0, 0);
            webview.Control.Dock = DockStyle.Fill;
            Controls.Add(webview.Control);

            Text = config.Title;
            Width = config.Width;
            Height = config.Height;
            CanResize = config.CanResize;

            ColorTools.ParseHex(config.BackgroundColor, out byte r, out byte g, out byte b);
            BackColor = Color.FromArgb(r, g, b);

            if (config.EnableScriptInterface) { bridge.Init(this, webview, windowFactory); }

            if (config.UseBrowserTitle)
            {
                bridge.TitleChanged += (s, e) => Title = e ?? config.Title;
            }

            SetIcon(config.Icon);
        }

        public void LoadUrl(string url)
        {
            webview.NavigateToFile(url);
        }

        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case UI.WindowState.Normal:
                    WindowState = FormWindowState.Normal;
                    break;

                case UI.WindowState.Maximized:
                    WindowState = FormWindowState.Maximized;
                    break;

                case UI.WindowState.Minimized:
                    WindowState = FormWindowState.Minimized;
                    break;

                default:
                    throw new ArgumentException($"Invalid window state of \"{state}\"", nameof(state));
            }
        }

        public void SetIcon(AppIcon icon)
        {
            if (icon == null || icon.Icons.Length == 0) { Icon = null; }
            else
            {
                using (var stream = icon.GetIconDataStream(icon.DefaultIcon))
                {
                    Icon = new Icon(stream);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var args = new CancelableEventArgs();
            ClosingBackingEvent?.Invoke(this, args);
            e.Cancel = args.Cancel;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ClosedBackingEvent?.Invoke(this, EventArgs.Empty);
            base.OnClosed(e);
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            webview.Dispose();
            server?.Dispose();
        }

        private bool IsEdgeAvailable()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll");
            var version = Native.GetOsVersion();

            return File.Exists(path) && version.MajorVersion >= 10 && version.BuildNumber >= 17134;
        }
    }
}
