using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using SpiderEye.Bridge;
using SpiderEye.Tools;
using SpiderEye.Windows.Interop;
using SDSize = System.Drawing.Size;

namespace SpiderEye.Windows
{
    internal class WinFormsWindow : Form, IWindow
    {
        event CancelableEventHandler? IWindow.Closing
        {
            add { ClosingBackingEvent += value; }
            remove { ClosingBackingEvent -= value; }
        }

        private event CancelableEventHandler? ClosingBackingEvent;

        public string? Title
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
            get { return canResizeField; }
            set
            {
                MaximizeBox = value;
                canResizeField = value;
                SetBorderStyle();
            }
        }

        public WindowBorderStyle BorderStyle
        {
            get { return borderStyleField; }
            set
            {
                borderStyleField = value;
                SetBorderStyle();
            }
        }

        public string? BackgroundColor
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

        AppIcon? IWindow.Icon
        {
            get { return icon; }
            set { SetIcon(value); }
        }

        public bool EnableScriptInterface
        {
            get { return webview.EnableScriptInterface; }
            set { webview.EnableScriptInterface = value; }
        }

        public bool EnableDevTools
        {
            get { return webview.EnableDevTools; }
            set { webview.EnableDevTools = value; }
        }

        public IWebview Webview
        {
            get { return webview; }
        }

        private readonly IWinFormsWebview webview;

        private AppIcon? icon;
        private bool canResizeField = true;
        private WindowBorderStyle borderStyleField;

        private WindowPlacement prevPlacement;
        private SDSize maxSize;

        public WinFormsWindow(WebviewBridge bridge)
        {
            if (bridge == null) { throw new ArgumentNullException(nameof(bridge)); }

            var webviewType = ChooseWebview();
            switch (webviewType)
            {
                case WebviewType.InternetExplorer:
                    webview = new IeLegacyWebview(WindowsApplication.ContentServerAddress, bridge);
                    break;

#if WINRT
                case WebviewType.Edge:
                    webview = new EdgeHtmlWebview(bridge);
                    break;
#endif

                case WebviewType.EdgeChromium:
                    var edgium = new EdgiumWebview(WindowsApplication.ContentServerAddress, bridge);
                    edgium.TitleChanged += Webview_TitleChanged;
                    webview = edgium;
                    break;

                default:
                    throw new InvalidOperationException($"Invalid webview type of {webviewType}");
            }

            // hide control until first page is loaded to prevent ugly flicker
            // Edgium and IE don't (properly) support setting a webview background color so this is a workaround
            webview.Control.Visible = false;
            webview.PageLoaded += Webview_PageLoaded;

            webview.Control.Location = new System.Drawing.Point(0, 0);
            webview.Control.Dock = DockStyle.Fill;
            Controls.Add(webview.Control);
        }

        public void EnterFullscreen()
        {
            var style = Native.GetWindowStyle(this);
            if (!IsFullscreen(style))
            {
                maxSize = MaximumSize;
                MaximumSize = SDSize.Empty;

                prevPlacement = Native.GetWindowPlacement(this);
                var mi = Native.GetMonitorInfo(this, Monitor.DefaultToNearest);
                Native.SetWindowStyle(this, style & ~WS.OVERLAPPEDWINDOW);
                Native.SetWindowPos(
                    this,
                    mi.Monitor.Left,
                    mi.Monitor.Top,
                    mi.Monitor.Right - mi.Monitor.Left,
                    mi.Monitor.Bottom - mi.Monitor.Top,
                    SWP.NOOWNERZORDER | SWP.FRAMECHANGED);
            }
        }

        public void ExitFullscreen()
        {
            var style = Native.GetWindowStyle(this);
            if (IsFullscreen(style))
            {
                Native.SetWindowStyle(this, style | WS.OVERLAPPEDWINDOW);
                Native.SetWindowPlacement(this, prevPlacement);
                Native.SetWindowPos(this, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOZORDER | SWP.NOOWNERZORDER | SWP.FRAMECHANGED);

                MaximumSize = maxSize;
            }
        }

        public void Maximize()
        {
            var style = Native.GetWindowStyle(this);
            if (IsFullscreen(style)) { prevPlacement.ShowCommand = SW.MAXIMIZE; }
            else { Native.SetWindowState(this, SW.MAXIMIZE); }
        }

        public void Unmaximize()
        {
            var style = Native.GetWindowStyle(this);
            if (IsFullscreen(style)) { prevPlacement.ShowCommand = SW.SHOWNORMAL; }
            else { Native.SetWindowState(this, SW.RESTORE); }
        }

        public void Minimize()
        {
            Native.SetWindowState(this, SW.MINIMIZE);
        }

        public void Unminimize()
        {
            var style = Native.GetWindowStyle(this);
            if (IsMinimized(style)) { Native.SetWindowState(this, SW.RESTORE); }
        }

        public void SetIcon(AppIcon? icon)
        {
            this.icon = icon;

            if (icon == null || icon.Icons.Length == 0) { Icon = null; }
            else
            {
                using var stream = icon.GetIconDataStream(icon.DefaultIcon);
                Icon = new Icon(stream);
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

        private static bool IsFullscreen(WS style)
        {
            return !style.HasFlag(WS.OVERLAPPEDWINDOW);
        }

        private static bool IsMinimized(WS style)
        {
            return style.HasFlag(WS.MINIMIZE);
        }

        private void Webview_PageLoaded(object sender, PageLoadEventArgs e)
        {
            if (!webview.Control.Visible)
            {
                webview.Control.Visible = true;
                webview.PageLoaded -= Webview_PageLoaded;
            }
        }

        private void Webview_TitleChanged(object? sender, string title)
        {
            if (UseBrowserTitle)
            {
                Application.Invoke(() => Title = title ?? string.Empty);
            }
        }

        private void SetBorderStyle()
        {
            FormBorderStyle = borderStyleField switch
            {
                WindowBorderStyle.Default => canResizeField ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle,
                WindowBorderStyle.None => FormBorderStyle.None,
                _ => throw new ArgumentException($"Invalid border style value of {borderStyleField}", nameof(BorderStyle)),
            };
        }

        private static WebviewType ChooseWebview()
        {
            switch (WindowsApplication.WebviewType)
            {
                case WebviewType.EdgeChromium:
                case WebviewType.Latest:
                    if (IsEdgiumAvailable()) { return WebviewType.EdgeChromium; }
#if WINRT
                    else { goto case WebviewType.Edge; }
#else
                    else { return WebviewType.InternetExplorer; }
#endif

#if WINRT
                case WebviewType.Edge:
                    if (IsEdgeAvailable()) { return WebviewType.Edge; }
                    else { return WebviewType.InternetExplorer; }
#endif

                case WebviewType.InternetExplorer:
                    return WebviewType.InternetExplorer;

                default:
                    return WindowsApplication.WebviewType;
            }
        }

        private static bool IsEdgiumAvailable()
        {
            string edgeVersion = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return !string.IsNullOrEmpty(edgeVersion);
        }

#if WINRT
        private static bool IsEdgeAvailable()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll");
            var version = Native.GetOsVersion();

            return System.IO.File.Exists(path) && version.MajorVersion >= 10 && version.BuildNumber >= 17763;
        }
#endif
    }
}
