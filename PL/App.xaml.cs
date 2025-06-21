using System.Configuration;
using System.Data;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // לדוגמה, תזין ID ידני לצורך בדיקה
            string volunteerId = "327725602"; // תחליף ל-ID אמיתי

            var window = new VolunteerMainWindow(volunteerId);
            window.Show();
        }

    }

}
