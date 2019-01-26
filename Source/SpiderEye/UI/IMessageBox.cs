namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a message box.
    /// </summary>
    public interface IMessageBox : IDialog
    {
        /// <summary>
        /// Gets or sets the title of the message box.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the displayed buttons.
        /// </summary>
        MessageBoxButton Buttons { get; set; }
    }
}
