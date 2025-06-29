using System.Windows;

namespace PL
{
    /// <summary>
    /// Provides a unified way to show user-friendly messages.
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Shows a message box with error icon.
        /// </summary>
        public static void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows a message box with information icon.
        /// </summary>
        public static void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows a message box with warning icon.
        /// </summary>
        public static void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Shows a Yes/No confirmation box and returns the result.
        /// </summary>
        public static bool ShowYesNo(string title, string question)
        {
            var result = MessageBox.Show(question, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
