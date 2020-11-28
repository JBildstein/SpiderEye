namespace SpiderEye
{
    /// <summary>
    /// Provides methods to show a message box.
    /// </summary>
    public static class MessageBox
    {
        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message">The displayed message.</param>
        /// <returns>The user selection.</returns>
        public static DialogResult Show(string message)
        {
            return Show(null, message, null, MessageBoxButtons.Ok);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message">The displayed message.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>The user selection.</returns>
        public static DialogResult Show(string message, string? title)
        {
            return Show(null, message, title, MessageBoxButtons.Ok);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message">The displayed message.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">The displayed buttons.</param>
        /// <returns>The user selection.</returns>
        public static DialogResult Show(string message, string? title, MessageBoxButtons buttons)
        {
            return Show(null, message, title, buttons);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <param name="message">The displayed message.</param>
        /// <returns>The user selection.</returns>
        public static DialogResult Show(Window? parent, string message)
        {
            return Show(parent, message, null, MessageBoxButtons.Ok);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <param name="message">The displayed message.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>The user selection.</returns>
        public static DialogResult Show(Window? parent, string message, string? title)
        {
            return Show(parent, message, title, MessageBoxButtons.Ok);
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <param name="message">The displayed message.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">The displayed buttons.</param>
        /// <returns>The user selection.</returns>
        public static DialogResult Show(Window? parent, string message, string? title, MessageBoxButtons buttons)
        {
            var msgBox = Application.Factory.CreateMessageBox();
            msgBox.Title = title;
            msgBox.Message = message;
            msgBox.Buttons = buttons;

            return msgBox.Show(parent?.NativeWindow);
        }
    }
}
