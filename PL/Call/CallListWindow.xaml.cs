using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl s_bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        private IEnumerable<CallInList> callsListView = new List<CallInList>();
        public IEnumerable<CallInList> CallsListView
        {
            get => callsListView;
            set
            {
                callsListView = value;
                OnPropertyChanged(nameof(CallsListView));
            }
        }

        public IEnumerable<CallTypeEnum> CallTypes => Enum.GetValues(typeof(CallTypeEnum)).Cast<CallTypeEnum>();

        private CallTypeEnum selectedCallType = CallTypeEnum.None;
        public CallTypeEnum SelectedCallType
        {
            get => selectedCallType;
            set
            {
                selectedCallType = value;
                OnPropertyChanged(nameof(SelectedCallType));
                LoadCalls();
            }
        }

        public CallInList? SelectedCall { get; set; }

        private readonly CallStatus? filterStatus;

        public CallListWindow(CallStatus? filterStatus = null)
        {
            InitializeComponent();
            this.filterStatus = filterStatus;
            DataContext = this;
            s_bl.Call.AddObserver(OnCallListChanged);
            LoadCalls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => LoadCalls();

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadCalls();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Call.RemoveObserver(OnCallListChanged);
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            s_bl.Call.RemoveObserver(OnCallListChanged);
        }

        private void LoadCalls()
        {
            try
            {
                var calls = s_bl.Call.GetCallList();

                if (SelectedCallType != CallTypeEnum.None)
                    calls = calls.Where(c => c.Type == SelectedCallType);

                if (filterStatus.HasValue)
                    calls = calls.Where(c => c.Status == filterStatus.Value);

                CallsListView = calls.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading calls: {ex.Message}");
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CallInList call)
            {
                try
                {
                    s_bl.Call.DeleteCall(call.Id);
                    LoadCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot delete call: " + ex.Message);
                }
            }
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is CallInList call)
            {
                try
                {
                    var fullCall = s_bl.Call.GetCallDetails(call.Id);
                    var latest = fullCall.Assignments?.LastOrDefault(a => a.CompletionTime == null);
                    if (latest != null && latest.VolunteerId.HasValue)
                    {
                        s_bl.Call.CancelCall(latest.VolunteerId.Value, latest.Id);
                        MessageBox.Show("Assignment cancelled.");
                        LoadCalls();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error cancelling assignment: " + ex.Message);
                }
            }
        }

        private void CallsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                var window = new SingleCallWindow(SelectedCall);
                window.Show();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new CallWindow();
            addWindow.Show();
        }

        private void OnCallListChanged() => Dispatcher.Invoke(LoadCalls);

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
