using System;
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
    public partial class VolunteerMainWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();

        public BO.Volunteer? Volunteer { get; set; }

        private BO.Call? currentCall;
        public BO.Call? CurrentCall
        {
            get => currentCall;
            set
            {
                currentCall = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCallInProgress));
                OnPropertyChanged(nameof(CanAssignCall));
                OnPropertyChanged(nameof(CanCancelCall));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool HasCallInProgress => CurrentCall != null;

        public bool CanAssignCall => !HasCallInProgress && Volunteer?.IsActive == true;

        public bool CanCancelCall => HasCallInProgress;

        public ICommand UpdateCommand { get; set; }
        public ICommand HistoryCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand FinishCommand { get; set; }
        public ICommand AssignCallCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private string mapLink;
        public string MapLink
        {
            get => mapLink;
            set
            {
                mapLink = value;
                OnPropertyChanged();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        /// <summary>
        /// construcer
        /// </summary>
        /// <param name="volunteerId"></param>
        public VolunteerMainWindow(string volunteerId)
        {
            InitializeComponent();

            try
            {
                Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
                if (int.TryParse(volunteerId, out int volunteerIdInt))
                    RefreshCall(volunteerIdInt);
                else
                    throw new ArgumentException("Volunteer ID is not a valid integer.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initialization failed: " + ex.Message);
                Close();
            }

            FinishCommand = new RelayCommand(_ => FinishCurrentCall(), _ => HasCallInProgress);
            UpdateCommand = new RelayCommand(_ => OpenUpdateWindow());
            HistoryCommand = new RelayCommand(_ => OpenHistoryWindow());
            LogoutCommand = new RelayCommand(_ => LogOut());
            AssignCallCommand = new RelayCommand(_ => OpenAssignCallWindow(), _ => CanAssignCall);
            CancelCommand = new RelayCommand(_ => CancelCall(), _ => CanCancelCall);

            DataContext = this;
        }

        /// <summary>
        /// refreshes calls after diferent actions
        /// </summary>
        /// <param name="volunteerId"></param>
        private void RefreshCall(int volunteerId)
        {
            try
            {
                var openCalls = _bl.Call.GetOpenCallsForVolunteer(volunteerId);
                var firstOpenCall = openCalls.FirstOrDefault();

                if (firstOpenCall != null)
                {
                    var call = _bl.Call.GetCallDetails(firstOpenCall.Id);
                    CurrentCall = call?.Status == CallStatus.Expired ? null : call;
                    
                }
                else
                {
                    CurrentCall = null;
                }

                if (CurrentCall != null && CurrentCall.Status != CallStatus.Expired)
                {
                    string origin = Uri.EscapeDataString(Volunteer.CurrentAddress);
                    string destination = Uri.EscapeDataString(CurrentCall.Address);
                    MapLink = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}";
                }
                else
                {
                    MapLink = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading open calls: " + ex.Message);
                CurrentCall = null;
                MapLink = string.Empty;
            }
        }

        /// <summary>
        /// a function that finishes the calls with treated status.
        /// </summary>
        private void FinishCurrentCall()
        {
            if (CurrentCall == null)
                return;

            try
            {
                int? assignmentId = _bl.Call.GetAssignmentId(CurrentCall.Id, Volunteer.Id);
                if (assignmentId == null)
                {
                    MessageBox.Show("No assignment found for this volunteer.");
                    return;
                }

                _bl.Call.CloseCall(Volunteer.Id, assignmentId.Value);
                MessageBox.Show($"Call {CurrentCall.Id} marked as Treated.");

                CurrentCall = null;
                RefreshCall(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to finish call: " + ex.Message);
            }
        }

        /// <summary>
        /// a function that ends the call with a canceld status.
        /// </summary>
        private void CancelCall()
        {
            if (CurrentCall == null)
                return;

            try
            {
                int? assignmentId = _bl.Call.GetAssignmentId(CurrentCall.Id, Volunteer.Id);
                if (assignmentId == null)
                {
                    MessageBox.Show("No assignment found to cancel.");
                    return;
                }

                _bl.Call.CancelCall(Volunteer.Id, assignmentId.Value);
                MessageBox.Show($"Call {CurrentCall.Id} was cancelled.");

                CurrentCall = null;
                RefreshCall(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to cancel call: " + ex.Message);
            }
        }

        /// <summary>
        /// a function that assgns a call to a specific volunteer.
        /// </summary>
        private void OpenAssignCallWindow()
        {
            try
            {
                new AssignCallWindow(Volunteer.Id).ShowDialog();
                RefreshCall(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open call assignment window: " + ex.Message,
                                "Assign Call Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// a func that logsout from the current volunteer.
        /// </summary>
        private void LogOut()
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

        /// <summary>
        /// a func that allows a volunteer to update it's details.
        /// </summary>
        private void OpenUpdateWindow()
        {
            new VolunteerWindow(Volunteer.Id.ToString()).ShowDialog();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id.ToString());
            OnPropertyChanged(nameof(Volunteer));
        }

        /// <summary>
        /// a func that opens the volunteers history calls window.
        /// </summary>
        private void OpenHistoryWindow()
        {
            new CallHistoryWindow(Volunteer.Id).ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
