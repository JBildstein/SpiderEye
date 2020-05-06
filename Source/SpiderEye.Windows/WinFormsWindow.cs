using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SpiderEye.Bridge;
using SpiderEye.Tools;
using SpiderEye.Windows.Interop;
using SDSize = System.Drawing.Size;

namespace SpiderEye.Windows
{
    internal class WinFormsWindow : Form, IWindow
    {
        event CancelableEventHandler IWindow.Closing
        {
            add { ClosingBackingEvent += value; }
            remove { ClosingBackingEvent -= value; }
        }

        private event CancelableEventHandler ClosingBackingEvent;

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        Size IWindow.Size
        {
            get { return new Size(Size.Width, Size.Height); }
            set { Size = new SDSize((int)value.Width, (int)value.Height); }
        }

        public Size MinSize
        {
            get { return new Size(MinimumSize.Width, MinimumSize.Height); }
            set { MinimumSize = new SDSize((int)value.Width, (int)value.Height); }
        }

        public Size MaxSize
        {
            get { return new Size(MaximumSize.Width, MaximumSize.Height); }
            set { MaximumSize = new SDSize((int)value.Width, (int)value.Height); }
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

        public string BackgroundColor
        {
            get { return ColorTools.ToHex(BackColor.R, BackColor.G, BackColor.B); }
            set
            {
                ColorTools.ParseHex(value, out byte r, out byte g, out byte b);
                BackColor = Color.FromArgb(r, g, b);
                webview.UpdateBackgroundColor(r, g, b);
            }
        }

        public bool UseBrowserTitle { get; set; }

        AppIcon IWindow.Icon
        {
            get { return icon; }
            set { SetIcon(value); }
        }

        public bool EnableScriptInterface
        {
            get { return webview.EnableScriptInterface; }
            set { webview.EnableScriptInterface = value; }
        }

        public bool EnableDevTools { get; set; }

        public IWebview Webview
        {
            get { return webview; }
        }
        
        object IWindow.NativeOptions => this;

        private readonly IWinFormsWebview webview;

        private AppIcon icon;

        public WinFormsWindow(WebviewBridge bridge)
        {
            if (bridge == null) { throw new ArgumentNullException(nameof(bridge)); }

            var webviewType = ChooseWebview();
            switch (webviewType)
            {
                case WebviewType.InternetExplorer:
                    webview = new WinFormsLegacyWebview(WindowsApplication.ContentServerAddress, bridge);
                    break;

                case WebviewType.Edge:
                    webview = new WinFormsWebview(bridge);
                    break;

                default:
                    throw new InvalidOperationException($"Invalid webview type of {webviewType}");
            }

            webview.Control.Location = new Point(0, 0);
            webview.Control.Dock = DockStyle.Fill;
            Controls.Add(webview.Control);
        }

        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case SpiderEye.WindowState.Normal:
                    WindowState = FormWindowState.Normal;
                    break;

                case SpiderEye.WindowState.Maximized:
                    WindowState = FormWindowState.Maximized;
                    break;

                case SpiderEye.WindowState.Minimized:
                    WindowState = FormWindowState.Minimized;
                    break;

                default:
                    throw new ArgumentException($"Invalid window state of \"{state}\"", nameof(state));
            }
        }

        public void SetIcon(AppIcon icon)
        {
            this.icon = icon;

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            webview.Dispose();
        }

        private WebviewType ChooseWebview()
        {
            switch (WindowsApplication.WebviewType)
            {
                case WebviewType.Edge:
                case WebviewType.Latest:
                    if (IsEdgeAvailable()) { return WebviewType.Edge; }
                    else { return WebviewType.InternetExplorer; }

                case WebviewType.InternetExplorer:
                    return WebviewType.InternetExplorer;

                default:
                    return WindowsApplication.WebviewType;
            }
        }

        private bool IsEdgeAvailable()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll");
            var version = WinNative.GetOsVersion();

            return File.Exists(path) && version.MajorVersion >= 10 && version.BuildNumber >= 17134;
        }
    }
}
