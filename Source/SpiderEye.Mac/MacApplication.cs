using System;
using System.Threading;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    /// <summary>
    /// Provides macOS specific application methods.
    /// </summary>
    public static class MacApplication
    {
        /// <summary>
        /// Gets or sets the application menu.
        /// </summary>
        public static Menu AppMenu
        {
            get { return appMenu; }
            set
            {
                appMenu = value;
                SetAppMenu(value);
            }
        }

        internal static IntPtr Handle
        {
            get { return app.Handle; }
        }

        internal static SynchronizationContext SynchronizationContext
        {
            get { return app.SynchronizationContext; }
        }

        private static CocoaApplication app;
        private static Menu appMenu;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Init()
        {
            app = new CocoaApplication();
            Application.Register(app, OperatingSystem.MacOS);
            AppMenu = CreateDefaultMenu();
        }

        private static void SetAppMenu(Menu menu)
        {
            var nativeMenu = NativeCast.To<CocoaMenu>(menu.NativeMenu);
            ObjC.Call(Handle, "setMainMenu:", nativeMenu?.Handle ?? IntPtr.Zero);
        }

        private static Menu CreateDefaultMenu()
        {
            var menu = new Menu();
            var appMenu = menu.MenuItems.AddLabelItem(string.Empty);
            appMenu.MenuItems.AddAboutItem();
            appMenu.MenuItems.AddSeparatorItem();
            appMenu.MenuItems.AddServicesMenu();
            appMenu.MenuItems.AddSeparatorItem();
            appMenu.MenuItems.AddHideItem();
            appMenu.MenuItems.AddHideOthersItem();
            appMenu.MenuItems.AddShowAllItem();
            appMenu.MenuItems.AddSeparatorItem();
            appMenu.MenuItems.AddQuitItem();

            var fileMenu = menu.MenuItems.AddLabelItem("File");
            fileMenu.MenuItems.AddNewDocumentItem();
            fileMenu.MenuItems.AddOpenDocumentItem();
            fileMenu.MenuItems.AddRecentDocumentsMenu();
            fileMenu.MenuItems.AddSeparatorItem();
            fileMenu.MenuItems.AddCloseDocumentItem();
            fileMenu.MenuItems.AddSaveDocumentItem();
            fileMenu.MenuItems.AddSaveAsDocumentItem();
            fileMenu.MenuItems.AddRevertToSavedDocumentItem();
            fileMenu.MenuItems.AddSeparatorItem();
            fileMenu.MenuItems.AddPageSetupItem();
            fileMenu.MenuItems.AddPrintItem();

            var editMenu = menu.MenuItems.AddLabelItem("Edit");
            editMenu.MenuItems.AddUndoItem();
            editMenu.MenuItems.AddRedoItem();
            editMenu.MenuItems.AddSeparatorItem();
            editMenu.MenuItems.AddCutItem();
            editMenu.MenuItems.AddCopyItem();
            editMenu.MenuItems.AddPasteItem();
            editMenu.MenuItems.AddPasteAndMatchStyleItem();
            editMenu.MenuItems.AddDeleteItem();
            editMenu.MenuItems.AddSelectAllItem();
            editMenu.MenuItems.AddSeparatorItem();
            editMenu.MenuItems.AddFindMenu();
            editMenu.MenuItems.AddSpellingAndGrammarMenu();
            editMenu.MenuItems.AddSubstitutionsMenu();
            editMenu.MenuItems.AddTransformationsMenu();
            editMenu.MenuItems.AddSpeechMenu();

            var viewMenu = menu.MenuItems.AddLabelItem("View");
            viewMenu.MenuItems.AddShowToolbarItem();
            viewMenu.MenuItems.AddCustomizeToolbarItem();
            viewMenu.MenuItems.AddSeparatorItem();
            viewMenu.MenuItems.AddShowSidebarItem();
            viewMenu.MenuItems.AddEnterFullScreenItem();

            var windowMenu = menu.MenuItems.AddLabelItem("Window");
            windowMenu.MenuItems.AddMinimizeItem();
            windowMenu.MenuItems.AddZoomItem();
            windowMenu.MenuItems.AddSeparatorItem();
            windowMenu.MenuItems.AddBringAllToFrontItem();

            var helpMenu = menu.MenuItems.AddLabelItem("Help");
            helpMenu.MenuItems.AddHelpItem();

            return menu;
        }
    }
}
