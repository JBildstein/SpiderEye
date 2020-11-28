using System;
using SpiderEye.Bridge;

namespace SpiderEye
{
    /// <summary>
    /// Represents a window.
    /// </summary>
    public sealed class Window : IDisposable
    {
        /// <summary>
        /// Fires before the webview navigates to an new URL.
        /// </summary>
        public event NavigatingEventHandler Navigating
        {
            add { NativeWindow.Webview.Navigating += value; }
            remove { NativeWindow.Webview.Navigating -= value; }
        }

        /// <summary>
        /// Fires once the content in the webview has loaded.
        /// </summary>
        public event PageLoadEventHandler PageLoaded
        {
            add { NativeWindow.Webview.PageLoaded += value; }
            remove { NativeWindow.Webview.PageLoaded -= value; }
        }

        /// <summary>
        /// Fires when the window is shown.
        /// </summary>
        public event EventHandler Shown
        {
            add { NativeWindow.Shown += value; }
            remove { NativeWindow.Shown -= value; }
        }

        /// <summary>
        /// Fires before the window gets closed.
        /// </summary>
        public event CancelableEventHandler Closing
        {
            add { NativeWindow.Closing += value; }
            remove { NativeWindow.Closing -= value; }
        }

        /// <summary>
        /// Fires after the window has closed.
        /// </summary>
        public event EventHandler Closed
        {
            add { NativeWindow.Closed += value; }
            remove { NativeWindow.Closed -= value; }
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string? Title
        {
            get { return NativeWindow.Title; }
            set { NativeWindow.Title = value; }
        }

        /// <summary>
        /// Gets or sets the window size.
        /// </summary>
        public Size Size
        {
            get { return NativeWindow.Size; }
            set { NativeWindow.Size = value; }
        }

        /// <summary>
        /// Gets or sets the minimum window size.
        /// </summary>
        public Size MinSize
        {
            get { return NativeWindow.MinSize; }
            set { NativeWindow.MinSize = value; }
        }

        /// <summary>
        /// Gets or sets the maximum window size. Set to <see cref="Size.Zero"/> to reset.
        /// </summary>
        public Size MaxSize
        {
            get { return NativeWindow.MaxSize; }
            set { NativeWindow.MaxSize = value; }
        }

        /// <summary>
        /// Gets or sets the border style of the window.
        /// </summary>
        public WindowBorderStyle BorderStyle
        {
            get { return NativeWindow.BorderStyle; }
            set { NativeWindow.BorderStyle = value; }
        }

        /// <summary>
        /// Gets or sets the background color of the window.
        /// </summary>
        public string? BackgroundColor
        {
            get { return NativeWindow.BackgroundColor; }
            set
            {
                if (string.IsNullOrEmpty(value)) { value = "#FFFFFF"; }
                NativeWindow.BackgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window can be resized or not.
        /// </summary>
        public bool CanResize
        {
            get { return NativeWindow.CanResize; }
            set { NativeWindow.CanResize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window title should
        /// be updated when the browser title does.
        /// </summary>
        public bool UseBrowserTitle
        {
            get { return NativeWindow.UseBrowserTitle; }
            set { NativeWindow.UseBrowserTitle = value; }
        }

        /// <summary>
        /// Gets or sets the default window icon. Default is null.
        /// </summary>
        public AppIcon? Icon
        {
            get { return NativeWindow.Icon; }
            set { NativeWindow.Icon = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the scripting interface between
        /// browser and window is enabled.
        /// </summary>
        public bool EnableScriptInterface
        {
            get { return NativeWindow.EnableScriptInterface; }
            set { NativeWindow.EnableScriptInterface = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dev tools are enabled or not.
        /// Note that this has a different or no effect depending on the used webview.
        /// </summary>
        public bool EnableDevTools
        {
            get { return NativeWindow.EnableDevTools; }
            set { NativeWindow.EnableDevTools = value; }
        }

        /// <summary>
        /// Gets a bridge to the webview.
        /// </summary>
        public IWebviewBridge Bridge
        {
            get { return bridge; }
        }

        /// <summary>
        /// Gets the native window.
        /// </summary>
        internal IWindow NativeWindow { get; }

        internal static readonly WindowConfiguration DefaultConfig = new WindowConfiguration();

        private readonly WebviewBridge bridge;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
        {
            bridge = new WebviewBridge(this);
            NativeWindow = Application.Factory.CreateWindow(DefaultConfig, bridge);

            Title = DefaultConfig.Title;
            Size = DefaultConfig.Size;
            MinSize = DefaultConfig.MinSize;
            MaxSize = DefaultConfig.MaxSize;
            BackgroundColor = DefaultConfig.BackgroundColor;
            CanResize = DefaultConfig.CanResize;
            EnableScriptInterface = DefaultConfig.EnableScriptInterface;
            UseBrowserTitle = DefaultConfig.UseBrowserTitle;
            EnableDevTools = DefaultConfig.EnableDevTools;

            bridge.TitleChanged += Bridge_TitleChanged;
            NativeWindow.Shown += NativeWindow_Shown;
            NativeWindow.Closed += NativeWindow_Closed;
        }

        /// <summary>
        /// Shows this window.
        /// </summary>
        public void Show()
        {
            NativeWindow.Show();
        }

        /// <summary>
        /// Closes this window.
        /// </summary>
        public void Close()
        {
            NativeWindow.Close();
        }

        /// <summary>
        /// Puts the window into full screen mode.
        /// </summary>
        public void EnterFullscreen()
        {
            NativeWindow.EnterFullscreen();
        }

        /// <summary>
        /// Exits full screen mode if the window is in full screen mode.
        /// </summary>
        public void ExitFullscreen()
        {
            NativeWindow.ExitFullscreen();
        }

        /// <summary>
        /// Maximizes the window.
        /// </summary>
        public void Maximize()
        {
            NativeWindow.Maximize();
        }

        /// <summary>
        /// Restores the window size if it was maximized.
        /// </summary>
        public void Unmaximize()
        {
            NativeWindow.Unmaximize();
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        public void Minimize()
        {
            NativeWindow.Minimize();
        }

        /// <summary>
        /// Restores the window if it was minimized.
        /// </summary>
        public void Unminimize()
        {
            NativeWindow.Unminimize();
        }

        /// <summary>
        /// Loads the given URL.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        public void LoadUrl(string url)
        {
            if (url == null) { throw new ArgumentNullException(nameof(url)); }

            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            uri = Application.UriWatcher.CheckUri(uri);
            NativeWindow.Webview.LoadUri(uri);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            NativeWindow.Dispose();
        }

        private void Bridge_TitleChanged(object? sender, string title)
        {
            if (UseBrowserTitle)
            {
                Application.Invoke(() => Title = title ?? string.Empty);
            }
        }

        private void NativeWindow_Shown(object? sender, EventArgs e)
        {
            NativeWindow.Shown -= NativeWindow_Shown;
            Application.OpenWindows.Add(this);
        }

        private void NativeWindow_Closed(object? sender, EventArgs e)
        {
            bridge.TitleChanged -= Bridge_TitleChanged;
            NativeWindow.Closed -= NativeWindow_Closed;
        }
    }
}
