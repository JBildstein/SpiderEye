﻿using System;
using SpiderEye.Bridge;

namespace SpiderEye
{
    /// <summary>
    /// Represents a window.
    /// </summary>
    public sealed class Window : IDisposable
    {
        private bool hasInitiallyShown;

        /// <summary>
        /// Fires before the webview navigates to an new URL.
        /// </summary>
        public event NavigatingEventHandler Navigating;

        /// <summary>
        /// Fires once the content in the webview has loaded.
        /// </summary>
        public event PageLoadEventHandler PageLoaded;

        /// <summary>
        /// Fires when the window is shown.
        /// </summary>
        public event EventHandler Shown;

        /// <summary>
        /// Fires before the window gets closed.
        /// </summary>
        public event CancelableEventHandler Closing;

        /// <summary>
        /// Fires after the window has closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string Title
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
        /// Gets or sets the background color of the window.
        /// </summary>
        public string BackgroundColor
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
        public AppIcon Icon
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
        /// Gets or sets the window menu.
        /// </summary>
        public Menu Menu
        {
            get => NativeWindow.Menu;
            set => NativeWindow.Menu = value;
        }

        /// <summary>
        /// Gets mac os related window options.
        /// </summary>
        public IMacOsWindowOptions MacOsOptions => NativeWindow.NativeOptions as IMacOsWindowOptions;

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

            NativeWindow.Webview.PageLoaded += NativeWindow_PageLoaded;
            NativeWindow.Webview.Navigating += NativeWindow_Navigating;

            NativeWindow.Shown += NativeWindow_Shown;
            NativeWindow.Closed += NativeWindow_Closed;
            NativeWindow.Closing += NativeWindow_Closing;
        }

        /// <summary>
        /// Shows this window.
        /// </summary>
        public void Show()
        {
            NativeWindow.Show();
        }

        /// <summary>
        /// Shows a modal window. This method blocks until the modal is closed.
        /// </summary>
        /// <param name="modal">The modal window.</param>
        public void ShowModal(Window modal)
        {
            NativeWindow.ShowModal(modal.NativeWindow);
        }

        /// <summary>
        /// Closes this window.
        /// </summary>
        public void Close()
        {
            NativeWindow.Close();
        }

        /// <summary>
        /// Sets the window state.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetWindowState(WindowState state)
        {
            NativeWindow.SetWindowState(state);
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

        private void Bridge_TitleChanged(object sender, string title)
        {
            if (UseBrowserTitle)
            {
                Application.Invoke(() => Title = title ?? string.Empty);
            }
        }

        private void NativeWindow_PageLoaded(object sender, PageLoadEventArgs e)
        {
            PageLoaded?.Invoke(this, e);
        }

        private void NativeWindow_Navigating(object sender, NavigatingEventArgs e)
        {
            Navigating?.Invoke(this, e);
        }

        private void NativeWindow_Shown(object sender, EventArgs e)
        {
            Shown?.Invoke(this, e);

            if (hasInitiallyShown)
            {
                return;
            }

            Application.OpenWindows.Add(this);
            hasInitiallyShown = true;
        }

        private void NativeWindow_Closing(object sender, CancelableEventArgs e)
        {
            Closing?.Invoke(this, e);
        }

        private void NativeWindow_Closed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, e);

            bridge.TitleChanged -= Bridge_TitleChanged;

            NativeWindow.Closed -= NativeWindow_Closed;
            NativeWindow.Closing -= NativeWindow_Closing;
            NativeWindow.Shown -= NativeWindow_Shown;

            NativeWindow.Webview.PageLoaded -= NativeWindow_PageLoaded;
            NativeWindow.Webview.Navigating -= NativeWindow_Navigating;
        }
    }
}
