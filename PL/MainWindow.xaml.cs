using System;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;
/*using PL.Volunteer;*/

namespace PL
{
    public partial class MainWindow : Window
    {
        // אובייקט גישה לשכבת BL
        static readonly IBl s_bl = Factory.Get();

        // תכונה המייצגת את זמן השעון המוצג
        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(DateTime), typeof(MainWindow));

        // תכונת תלות עבור משתנה תצורה MaxYearRange מסוג TimeSpan
        public TimeSpan MaxYearRange
        {
            get => (TimeSpan)GetValue(MaxYearRangeProperty);
            set => SetValue(MaxYearRangeProperty, value);
        }
        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register(nameof(MaxYearRange), typeof(TimeSpan), typeof(MainWindow));

        public MainWindow()
        {
            // InitializeComponent();

            // הגדרת אירועים
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        // משקיף לשעון
        private void ClockObserver()
        {
            Dispatcher.Invoke(() =>
            {
                CurrentTime = s_bl.Admin.GetSystemClock();
            });
        }

        // משקיף לתצורה
        private void ConfigObserver()
        {
            Dispatcher.Invoke(() =>
            {
                MaxYearRange = s_bl.Admin.GetRiskTimeRange();
            });
        }

        // טעינת המסך
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }

        // סגירת המסך
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

        // מימוש כפתורי קידום שעון
        private void BtnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(TimeUnit.MINUTE);
        }

        private void BtnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(TimeUnit.HOUR);
        }

        private void BtnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(TimeUnit.DAY);
        }

        private void BtnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(TimeUnit.MONTH);
        }


        private void BtnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(TimeUnit.YEAR);
        }

        // עדכון משתנה תצורה
        private void BtnUpdateMaxYearRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetRiskTimeRange(MaxYearRange);
                MessageBox.Show("Max Year Range updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating Max Year Range: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // פתיחת מסך תצוגת הרשימה (לדוגמה)
        private void BtnCourses_Click(object sender, RoutedEventArgs e)
        {
            // new VolunteersListWindow().Show();
        }

        private async void BtnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialize", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // סגור את כל החלונות חוץ מהמסך הראשי
                foreach (Window w in Application.Current.Windows)
                {
                    if (w != this)
                        w.Close();
                }

                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    // תן ל-WPF זמן לעדכן את הסמן
                    await Task.Delay(100);

                    // אתחול המסד, רץ ברקע כדי לא לחסום את ה-UI
                    await Task.Run(() => s_bl.Admin.InitializeDatabase());

                    MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error initializing database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        // איפוס בסיס נתונים
        private void BtnResetDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database? This will delete all data!", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.ResetDatabase();
                    MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error resetting database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
        private void BtnVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new PL.Volunteer.VolunteersListWindow().Show();
        }


    }
}