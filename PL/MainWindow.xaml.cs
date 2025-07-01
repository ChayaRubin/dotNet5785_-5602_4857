using System;
using System.Collections.Generic;
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

        public ICommand? OpenStatusFilteredCallsCommand { get; private set; }

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

        public Dictionary<CallStatus, int> CallStatusCounts
        {
            get => _callStatusCounts;
            set
            {
                _callStatusCounts = value;
                OnPropertyChanged();
            }
        }
        private Dictionary<CallStatus, int> _callStatusCounts = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            OpenStatusFilteredCallsCommand = new RelayCommand(param =>
            {
                if (param is CallStatus status)
                    new PL.Call.CallListWindow(status).Show();
            });

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
            MaxYearRange = s_bl.Admin.GetRiskTimeRange();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
            s_bl.Call.AddObserver(UpdateStatusCounts);

            UpdateStatusCounts();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
            s_bl.Call.RemoveObserver(UpdateStatusCounts);

            if (IsSimulatorRunning)
                s_bl.Admin.StopSimulator();
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

        private void UpdateStatusCounts()
        {
            try
            {
                CallStatusCounts = s_bl.Call.GetCallCountsByStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating call counts: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginWindow = new LoginWindow();
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
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
