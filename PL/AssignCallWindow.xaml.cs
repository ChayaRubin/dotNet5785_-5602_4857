using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using BlApi;
using BO;

namespace PL
{
    public partial class AssignCallWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();

        public ObservableCollection<OpenCallInList> AvailableCalls { get; set; } = new();

        private OpenCallInList selectedCall;
        public OpenCallInList SelectedCall
        {
            get => selectedCall;
            set
            {
                selectedCall = value;
                UpdateMapLink();
                OnPropertyChanged();
            }
        }

        public ICommand AssignCallCommand { get; set; }

        private string mapLink;
        public string MapLink
        {
            get => mapLink;
            set { mapLink = value; OnPropertyChanged(); }
        }

        private readonly int volunteerId;
        private BO.Volunteer volunteer;

        public AssignCallWindow(int volunteerId)
        {
            InitializeComponent();
            DataContext = this;
            this.volunteerId = volunteerId;
            AssignCallCommand = new RelayCommand(AssignCall);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId.ToString());

                var openCalls = _bl.Call.GetAvailableOpenCalls(volunteerId);
                AvailableCalls.Clear();

                foreach (var call in openCalls)
                {
                    AvailableCalls.Add(call);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AssignCall(object parameter)
        {
            if (parameter is not OpenCallInList call)
            {
                MessageBox.Show("Invalid selection. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _bl.Call.AssignCall(volunteerId, call.Id);
                MessageBox.Show($"Call {call.Id} assigned successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to assign call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateMapLink()
        {
            if (SelectedCall == null || volunteer == null)
            {
                MapLink = string.Empty;
                return;
            }

            string origin = Uri.EscapeDataString(volunteer.CurrentAddress ?? "");
            string destination = Uri.EscapeDataString(SelectedCall.FullAddress);
            MapLink = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
