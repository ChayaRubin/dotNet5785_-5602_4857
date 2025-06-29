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
        public double? AssignedCallDistance { get; private set; }

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
        public ICommand UpdateAddressCommand { get; set; }

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
            UpdateAddressCommand = new RelayCommand(_ => UpdateAddress());

            LoadData();
        }

        public List<CallTypeEnum?> CallTypes { get; } = new List<CallTypeEnum?> { null }
            .Concat(Enum.GetValues(typeof(CallTypeEnum)).Cast<CallTypeEnum?>())
            .ToList();

        private CallTypeEnum? selectedCallTypeFilter = null;
        public CallTypeEnum? SelectedCallTypeFilter
        {
            get => selectedCallTypeFilter;
            set
            {
                selectedCallTypeFilter = value;
                OnPropertyChanged();
                FilterAndSortCalls();
            }
        }

        public List<string> SortOptions { get; } = new List<string>
        {
            "Distance",
            "Call ID",
            "Address"
        };

        private string selectedSortOption = "Distance";
        public string SelectedSortOption
        {
            get => selectedSortOption;
            set
            {
                selectedSortOption = value;
                OnPropertyChanged();
                FilterAndSortCalls();
            }
        }

        private string volunteerAddress;
        public string VolunteerAddress
        {
            get => volunteerAddress;
            set
            {
                volunteerAddress = value;
                OnPropertyChanged();
            }
        }

        private List<OpenCallInList> allCalls = new List<OpenCallInList>();

        private void LoadData()
        {
            try
            {
                volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId.ToString());
                VolunteerAddress = volunteer.CurrentAddress;

                allCalls = _bl.Call.GetAvailableOpenCalls(volunteerId).ToList();

                FilterAndSortCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterAndSortCalls()
        {
            var filtered = allCalls.AsEnumerable();

            if (SelectedCallTypeFilter.HasValue && SelectedCallTypeFilter.Value != CallTypeEnum.None)
            {
                filtered = filtered.Where(c => c.CallType == SelectedCallTypeFilter.Value);
            }


            filtered = SelectedSortOption switch
            {
                "Distance" => filtered.OrderBy(c => c.DistanceFromVolunteer),
                "Call ID" => filtered.OrderBy(c => c.Id),
                "Address" => filtered.OrderBy(c => c.FullAddress),
                _ => filtered.OrderBy(c => c.Id)
            };

            AvailableCalls.Clear();
            foreach (var call in filtered)
                AvailableCalls.Add(call);
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
                AssignedCallDistance = call.DistanceFromVolunteer;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to assign call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAddress()
        {
            if (string.IsNullOrWhiteSpace(VolunteerAddress))
            {
                MessageBox.Show("Address cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                volunteer.CurrentAddress = VolunteerAddress;
                _bl.Volunteer.UpdateVolunteerDetails(volunteer.Id.ToString(), volunteer);

                allCalls = _bl.Call.GetAvailableOpenCalls(volunteerId).ToList();

                FilterAndSortCalls();

                MessageBox.Show("Address updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update address: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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