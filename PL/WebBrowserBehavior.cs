using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public static class WebBrowserBehavior
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached(
                "BindableSource",
                typeof(string),
                typeof(WebBrowserBehavior),
                new UIPropertyMetadata(null, OnBindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        private static void OnBindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is WebBrowser webBrowser)
            {
                string uri = e.NewValue as string ?? "about:blank";
                webBrowser.Navigate(uri);
            }
        }
    }
}
