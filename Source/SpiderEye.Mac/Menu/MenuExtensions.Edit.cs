namespace SpiderEye.Mac
{
    /// <content>
    /// Edit menu extensions.
    /// </content>
    public static partial class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item for the undo action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddUndoItem(this MenuItemCollection menuItems, string label = "Undo")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "undo:", ModifierKey.Super, Key.Z);
        }

        /// <summary>
        /// Adds a menu item for the redo action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddRedoItem(this MenuItemCollection menuItems, string label = "Redo")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "redo:", ModifierKey.Super | ModifierKey.Shift, Key.Z);
        }

        /// <summary>
        /// Adds a menu item for the cut text action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCutItem(this MenuItemCollection menuItems, string label = "Cut")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "cut:", ModifierKey.Super, Key.X);
        }

        /// <summary>
        /// Adds a menu item for the copy text action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCopyItem(this MenuItemCollection menuItems, string label = "Copy")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "copy:", ModifierKey.Super, Key.C);
        }

        /// <summary>
        /// Adds a menu item for the paste text action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddPasteItem(this MenuItemCollection menuItems, string label = "Paste")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "paste:", ModifierKey.Super, Key.V);
        }

        /// <summary>
        /// Adds a menu item for the paste text and match style action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddPasteAndMatchStyleItem(this MenuItemCollection menuItems, string label = "Paste and Match Style")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "pasteAsPlainText:", ModifierKey.Super | ModifierKey.Shift, Key.V);
        }

        /// <summary>
        /// Adds a menu item for the delete text action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddDeleteItem(this MenuItemCollection menuItems, string label = "Delete")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "delete:");
        }

        /// <summary>
        /// Adds a menu item for the select all text action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSelectAllItem(this MenuItemCollection menuItems, string label = "Select All")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "selectAll:", ModifierKey.Super, Key.A);
        }

        /// <summary>
        /// Adds a menu for the various find actions.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddFindMenu(this MenuItemCollection menuItems, string label = "Find")
        {
            var menu = menuItems.AddLabelItem(label);
            menu.MenuItems.AddFindItem();
            menu.MenuItems.AddFindAndReplaceItem();
            menu.MenuItems.AddFindNextItem();
            menu.MenuItems.AddFindPreviousItem();
            menu.MenuItems.AddUseSelectionForFindItem();
            menu.MenuItems.AddJumpToSelectionItem();

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the find action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddFindItem(this MenuItemCollection menuItems, string label = "Find…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performFindPanelAction:", ModifierKey.Super, Key.F, 1);
        }

        /// <summary>
        /// Adds a menu item for the find action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddFindAndReplaceItem(this MenuItemCollection menuItems, string label = "Find and Replace…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performFindPanelAction:", ModifierKey.Super | ModifierKey.Alt, Key.F, 12);
        }

        /// <summary>
        /// Adds a menu item for the find action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddFindNextItem(this MenuItemCollection menuItems, string label = "Find Next")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performFindPanelAction:", ModifierKey.Super, Key.G, 2);
        }

        /// <summary>
        /// Adds a menu item for the find action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddFindPreviousItem(this MenuItemCollection menuItems, string label = "Find Previous")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performFindPanelAction:", ModifierKey.Super | ModifierKey.Shift, Key.G, 3);
        }

        /// <summary>
        /// Adds a menu item for the find action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddUseSelectionForFindItem(this MenuItemCollection menuItems, string label = "Use Selection for Find")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performFindPanelAction:", ModifierKey.Super, Key.E, 7);
        }

        /// <summary>
        /// Adds a menu item for the find action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddJumpToSelectionItem(this MenuItemCollection menuItems, string label = "Jump to Selection")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "centerSelectionInVisibleArea:", ModifierKey.Super, Key.J);
        }

        /// <summary>
        /// Adds a menu for the various find actions.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSpellingAndGrammarMenu(this MenuItemCollection menuItems, string label = "Spelling and Grammar")
        {
            var menu = menuItems.AddLabelItem(label);
            menu.MenuItems.AddShowSpellingAndGrammarItem();
            menu.MenuItems.AddCheckDocumentNowItem();
            menu.MenuItems.AddSeparatorItem();
            menu.MenuItems.AddCheckSpellingWhileTypingItem();
            menu.MenuItems.AddCheckGrammarWithSpellingItem();
            menu.MenuItems.AddCorrectSpellingAutomaticallyItem();

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the show spelling and grammar panel action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddShowSpellingAndGrammarItem(this MenuItemCollection menuItems, string label = "Show Spelling and Grammar")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "showGuessPanel:");
        }

        /// <summary>
        /// Adds a menu item for the check document spelling action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCheckDocumentNowItem(this MenuItemCollection menuItems, string label = "Check Document Now")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "checkSpelling:");
        }

        /// <summary>
        /// Adds a menu item for the continuous spelling toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCheckSpellingWhileTypingItem(this MenuItemCollection menuItems, string label = "Check Spelling While Typing")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleContinuousSpellChecking:");
        }

        /// <summary>
        /// Adds a menu item for the grammar checking toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCheckGrammarWithSpellingItem(this MenuItemCollection menuItems, string label = "Check Grammar With Spelling")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleGrammarChecking:");
        }

        /// <summary>
        /// Adds a menu item for the automatic spelling correction toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCorrectSpellingAutomaticallyItem(this MenuItemCollection menuItems, string label = "Correct Spelling Automatically")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleAutomaticSpellingCorrection:");
        }

        /// <summary>
        /// Adds a menu for the various substitution actions.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSubstitutionsMenu(this MenuItemCollection menuItems, string label = "Substitutions")
        {
            var menu = menuItems.AddLabelItem(label);
            menu.MenuItems.AddShowSubstitutionsItem();
            menu.MenuItems.AddSeparatorItem();
            menu.MenuItems.AddSmartCopyPasteItem();
            menu.MenuItems.AddSmartQuotesItem();
            menu.MenuItems.AddSmartDashesItem();
            menu.MenuItems.AddSmartLinksItem();
            menu.MenuItems.AddDataDetectorsItem();
            menu.MenuItems.AddTextReplacementItem();

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the show substitutions panel action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddShowSubstitutionsItem(this MenuItemCollection menuItems, string label = "Show Substitutions")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "orderFrontSubstitutionsPanel:");
        }

        /// <summary>
        /// Adds a menu item for the smart insert toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSmartCopyPasteItem(this MenuItemCollection menuItems, string label = "Smart Copy/Paste")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleSmartInsertDelete:");
        }

        /// <summary>
        /// Adds a menu item for the automatic quote substitution toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSmartQuotesItem(this MenuItemCollection menuItems, string label = "Smart Quotes")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleAutomaticQuoteSubstitution:");
        }

        /// <summary>
        /// Adds a menu item for the automatic dash substitution toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSmartDashesItem(this MenuItemCollection menuItems, string label = "Smart Dashes")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleAutomaticDashSubstitution:");
        }

        /// <summary>
        /// Adds a menu item for the automatic link detection toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSmartLinksItem(this MenuItemCollection menuItems, string label = "Smart Links")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleAutomaticLinkDetection:");
        }

        /// <summary>
        /// Adds a menu item for the automatic data detection toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddDataDetectorsItem(this MenuItemCollection menuItems, string label = "Data Detectors")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleAutomaticDataDetection:");
        }

        /// <summary>
        /// Adds a menu item for the automatic text replacement toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddTextReplacementItem(this MenuItemCollection menuItems, string label = "Text Replacement")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleAutomaticTextReplacement:");
        }

        /// <summary>
        /// Adds a menu for the various text transformation actions.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddTransformationsMenu(this MenuItemCollection menuItems, string label = "Transformations")
        {
            var menu = menuItems.AddLabelItem(label);
            menu.MenuItems.AddMakeUpperCaseItem();
            menu.MenuItems.AddMakeLowerCaseItem();
            menu.MenuItems.AddCapitalizeItem();

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the uppercase word action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddMakeUpperCaseItem(this MenuItemCollection menuItems, string label = "Make Upper Case")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "uppercaseWord:");
        }

        /// <summary>
        /// Adds a menu item for the lowercase word action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddMakeLowerCaseItem(this MenuItemCollection menuItems, string label = "Make Lower Case")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "lowercaseWord:");
        }

        /// <summary>
        /// Adds a menu item for the capitalize word action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCapitalizeItem(this MenuItemCollection menuItems, string label = "Capitalize")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "capitalizeWord:");
        }

        /// <summary>
        /// Adds a menu for the various speech actions.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSpeechMenu(this MenuItemCollection menuItems, string label = "Speech")
        {
            var menu = menuItems.AddLabelItem(label);
            menu.MenuItems.AddStartSpeakingItem();
            menu.MenuItems.AddStopSpeakingItem();

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the start speech action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddStartSpeakingItem(this MenuItemCollection menuItems, string label = "Start Speaking")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "startSpeaking:");
        }

        /// <summary>
        /// Adds a menu item for the stop speech action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddStopSpeakingItem(this MenuItemCollection menuItems, string label = "Stop Speaking")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "stopSpeaking:");
        }
    }
}
