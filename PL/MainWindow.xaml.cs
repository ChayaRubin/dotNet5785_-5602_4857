/*using System;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;
using System.Linq; // נוספה לצורך ספירת סטטוסים

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        private volatile bool _clockObserverWorking = false;
        private volatile bool _configObserverWorking = false;

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(DateTime), typeof(MainWindow));

        public TimeSpan MaxYearRange
        {
            get => (TimeSpan)GetValue(MaxYearRangeProperty);
            set => SetValue(MaxYearRangeProperty, value);
        }
        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register(nameof(MaxYearRange), typeof(TimeSpan), typeof(MainWindow));

        public int OpenCallsCount
        {
            get => (int)GetValue(OpenCallsCountProperty);
            set => SetValue(OpenCallsCountProperty, value);
        }
        public static readonly DependencyProperty OpenCallsCountProperty =
            DependencyProperty.Register(nameof(OpenCallsCount), typeof(int), typeof(MainWindow));

        public int InProgressCallsCount
        {
            get => (int)GetValue(InProgressCallsCountProperty);
            set => SetValue(InProgressCallsCountProperty, value);
        }
        public static readonly DependencyProperty InProgressCallsCountProperty =
            DependencyProperty.Register(nameof(InProgressCallsCount), typeof(int), typeof(MainWindow));

        public int ClosedCallsCount
        {
            get => (int)GetValue(ClosedCallsCountProperty);
            set => SetValue(ClosedCallsCountProperty, value);
        }
        public static readonly DependencyProperty ClosedCallsCountProperty =
            DependencyProperty.Register(nameof(ClosedCallsCount), typeof(int), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void ClockObserver()
        {
            if (_clockObserverWorking) return;
            _clockObserverWorking = true;

            _ = Dispatcher.BeginInvoke(() =>
            {
                CurrentTime = s_bl.Admin.GetSystemClock();
                _clockObserverWorking = false;
            });
        }


        private void ConfigObserver()
        {
            if (_configObserverWorking) return;
            _configObserverWorking = true;

            _ = Dispatcher.BeginInvoke(() =>
            {
                MaxYearRange = s_bl.Admin.GetRiskTimeRange();
                _configObserverWorking = false;
            });
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);

            UpdateStatusCounts();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

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

        private void BtnUpdateMaxYearRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetRiskTimeRange(MaxYearRange);
                ErrorHandler.ShowInfo("Success", "Max Year Range updated successfully.");
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError("Error Updating Max Year Range", ex.Message);
            }
        }

        private void BtnCourses_Click(object sender, RoutedEventArgs e)
        {
            // new VolunteersListWindow().Show();
        }

        private async void BtnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = ErrorHandler.ShowYesNo("Confirm Initialize", "Are you sure you want to initialize the database?");
            if (result)
            {
                foreach (Window w in Application.Current.Windows)
                {
                    if (w != this)
                        w.Close();
                }

                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    await Task.Delay(100);
                    await Task.Run(() => s_bl.Admin.InitializeDatabase());
                    ErrorHandler.ShowInfo("Success", "Database initialized successfully.");
                    UpdateStatusCounts();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Error Initializing Database", ex.Message);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void BtnResetDB_Click(object sender, RoutedEventArgs e)
        {
            var result = ErrorHandler.ShowYesNo("Confirm Reset", "Are you sure you want to reset the database? This will delete all data!");
            if (result)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.ResetDatabase();
                    ErrorHandler.ShowInfo("Success", "Database reset successfully.");
                    UpdateStatusCounts();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Error Resetting Database", ex.Message);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void BtnVolunteers_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<PL.Volunteer.VolunteersListWindow>().Any())
                new PL.Volunteer.VolunteersListWindow().Show();
        }

        private void BtnCalls_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<PL.Call.CallListWindow>().Any())
                new PL.Call.CallListWindow().Show();
        }

        // פונקציה חדשה לחישוב כמויות לפי סטטוסים
        private void UpdateStatusCounts()
        {
            var calls = s_bl.Call.GetCallList();
            OpenCallsCount = calls.Count(c => c.Status == CallStatus.Open);
            InProgressCallsCount = calls.Count(c => c.Status == CallStatus.InProgress);
            ClosedCallsCount = calls.Count(c => c.Status == CallStatus.Closed);
        }

        // לחצנים לפתיחת רשימות מסוננות לפי סטטוס
        private void BtnOpenCalls_Click(object sender, RoutedEventArgs e)
        {
            new PL.Call.CallListWindow(CallStatus.Open).Show();

        }

        private void BtnInProgressCalls_Click(object sender, RoutedEventArgs e)
        {
            new PL.Call.CallListWindow(CallStatus.InProgress).Show();

        }

        private void BtnClosedCalls_Click(object sender, RoutedEventArgs e)
        {
            new PL.Call.CallListWindow(CallStatus.Treated).Show();

        }

        /// <summary>
        /// a func that logsout from the current volunteer.
        /// </summary>

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError("Logout Failed", ex.Message);
            }
        }
    }
}
*/

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static readonly IBl s_bl = Factory.Get();

        private volatile bool _clockObserverWorking = false;
        private volatile bool _configObserverWorking = false;

        private bool isSimulatorRunning;
        public bool IsSimulatorRunning
        {
            get => isSimulatorRunning;
            set
            {
                isSimulatorRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SimulatorButtonText));
            }
        }

        private int interval = 1;
        public int Interval
        {
            get => interval;
            set
            {
                interval = value;
                OnPropertyChanged();
            }
        }

        public string SimulatorButtonText => IsSimulatorRunning ? "Stop Simulator" : "Start Simulator";

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(DateTime), typeof(MainWindow));

        public TimeSpan MaxYearRange
        {
            get => (TimeSpan)GetValue(MaxYearRangeProperty);
            set => SetValue(MaxYearRangeProperty, value);
        }
        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register(nameof(MaxYearRange), typeof(TimeSpan), typeof(MainWindow));

        public int OpenCallsCount
        {
            get => (int)GetValue(OpenCallsCountProperty);
            set => SetValue(OpenCallsCountProperty, value);
        }
        public static readonly DependencyProperty OpenCallsCountProperty =
            DependencyProperty.Register(nameof(OpenCallsCount), typeof(int), typeof(MainWindow));

        public int InProgressCallsCount
        {
            get => (int)GetValue(InProgressCallsCountProperty);
            set => SetValue(InProgressCallsCountProperty, value);
        }
        public static readonly DependencyProperty InProgressCallsCountProperty =
            DependencyProperty.Register(nameof(InProgressCallsCount), typeof(int), typeof(MainWindow));

        public int ClosedCallsCount
        {
            get => (int)GetValue(ClosedCallsCountProperty);
            set => SetValue(ClosedCallsCountProperty, value);
        }
        public static readonly DependencyProperty ClosedCallsCountProperty =
            DependencyProperty.Register(nameof(ClosedCallsCount), typeof(int), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void ClockObserver()
        {
            if (_clockObserverWorking) return;
            _clockObserverWorking = true;

            _ = Dispatcher.BeginInvoke(() =>
            {
                CurrentTime = s_bl.Admin.GetSystemClock();
                _clockObserverWorking = false;
            });
        }

        private void ConfigObserver()
        {
            if (_configObserverWorking) return;
            _configObserverWorking = true;

            _ = Dispatcher.BeginInvoke(() =>
            {
                MaxYearRange = s_bl.Admin.GetRiskTimeRange();
                _configObserverWorking = false;
            });
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);

            UpdateStatusCounts();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);

            if (IsSimulatorRunning)
                s_bl.Admin.StopSimulator();
        }

        private void BtnAddOneMinute_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceSystemClock(TimeUnit.MINUTE);
        private void BtnAddOneHour_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceSystemClock(TimeUnit.HOUR);
        private void BtnAddOneDay_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceSystemClock(TimeUnit.DAY);
        private void BtnAddOneMonth_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceSystemClock(TimeUnit.MONTH);
        private void BtnAddOneYear_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceSystemClock(TimeUnit.YEAR);

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

        private async void BtnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialize", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Window w in Application.Current.Windows)
                {
                    if (w != this)
                        w.Close();
                }

                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    await Task.Delay(100);
                    await Task.Run(() => s_bl.Admin.InitializeDatabase());
                    MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateStatusCounts();
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
                    UpdateStatusCounts();
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
            if (!Application.Current.Windows.OfType<PL.Volunteer.VolunteersListWindow>().Any())
                new PL.Volunteer.VolunteersListWindow().Show();
        }

        private void BtnCalls_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<PL.Call.CallListWindow>().Any())
                new PL.Call.CallListWindow().Show();
        }

        private void UpdateStatusCounts()
        {
            var calls = s_bl.Call.GetCallList();
            OpenCallsCount = calls.Count(c => c.Status == CallStatus.Open);
            InProgressCallsCount = calls.Count(c => c.Status == CallStatus.InProgress);
            ClosedCallsCount = calls.Count(c => c.Status == CallStatus.Closed);
        }

        private void BtnOpenCalls_Click(object sender, RoutedEventArgs e) => new PL.Call.CallListWindow(CallStatus.Open).Show();
        private void BtnInProgressCalls_Click(object sender, RoutedEventArgs e) => new PL.Call.CallListWindow(CallStatus.InProgress).Show();
        private void BtnClosedCalls_Click(object sender, RoutedEventArgs e) => new PL.Call.CallListWindow(CallStatus.Treated).Show();

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Logout failed: " + ex.Message);
            }
        }

        private void BtnStartStopSimulator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSimulatorRunning)
                {
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while toggling the simulator: " + ex.Message,
                                "Simulator Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}