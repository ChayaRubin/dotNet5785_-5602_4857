using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        /// <summary>
        /// The current call assigned to the volunteer.
        /// </summary>
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

        /// <summary>
        /// Constructor – initializes the volunteer window and loads data.
        /// </summary>
        /// <param name="volunteerId">The volunteer's ID as string.</param>
        public VolunteerMainWindow(string volunteerId)
        {
            InitializeComponent();

            if (!int.TryParse(volunteerId, out int volunteerIdInt))
            {
                ErrorHandler.ShowWarning("Invalid Volunteer ID", "Volunteer ID must be a number.");
                Close();
                return;
            }

            try
            {
                Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
                RefreshCall(volunteerIdInt);
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Initialization Failed", "Could not load volunteer data. Please check the ID and try again.");
                Close();
                return;
            }

            FinishCommand = new RelayCommand(_ => FinishCurrentCall(), _ => HasCallInProgress);
            UpdateCommand = new RelayCommand(_ => OpenUpdateWindow());
            HistoryCommand = new RelayCommand(_ => OpenHistoryWindow());
            LogoutCommand = new RelayCommand(_ => LogOut());
            AssignCallCommand = new RelayCommand(_ => OpenAssignCallWindow(), _ => CanAssignCall);
            CancelCommand = new RelayCommand(_ => CancelCall(), _ => CanCancelCall);
            DataContext = this;
        }

        private double? distanceToCall;
        public double? DistanceToCall
        {
            get => distanceToCall;
            set
            {
                distanceToCall = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Navigates to the Google Maps link when the user clicks the address link.
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Navigation Error", "Unable to open the map link in your browser.");
            }
        }

        /// <summary>
        /// Refreshes the current call assigned to the volunteer, and updates the map link.
        /// </summary>
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
            catch (Exception)
            {
                ErrorHandler.ShowError("Call Loading Error", "Could not load available calls. Please try again later.");
                CurrentCall = null;
                MapLink = string.Empty;
            }
        }

        /// <summary>
        /// Finishes the current call and marks it as Treated.
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
                    ErrorHandler.ShowWarning("Finish Call", "No assignment found for this volunteer.");
                    return;
                }

                _bl.Call.CloseCall(Volunteer.Id, assignmentId.Value);
                ErrorHandler.ShowInfo("Call Finished", $"Call {CurrentCall.Id} marked as Treated.");
                CurrentCall = null;
                RefreshCall(Volunteer.Id);
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Finish Call Failed", "Unable to finish the call. Please try again.");
            }
        }

        /// <summary>
        /// Cancels the current call assignment for the volunteer.
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
                    ErrorHandler.ShowWarning("Cancel Call", "No assignment found to cancel.");
                    return;
                }

                _bl.Call.CancelCall(Volunteer.Id, assignmentId.Value);
                ErrorHandler.ShowInfo("Call Cancelled", $"Call {CurrentCall.Id} was successfully cancelled.");
                CurrentCall = null;
                RefreshCall(Volunteer.Id);
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Cancel Call Failed", "Could not cancel the call. Please try again.");
            }
        }

        /// <summary>
        /// Opens a window for assigning a new call to the volunteer.
        /// </summary>
        private void OpenAssignCallWindow()
        {
            try
            {
                var assignWindow = new AssignCallWindow(Volunteer.Id);
                bool? result = assignWindow.ShowDialog();
                if (result == true)
                {
                    RefreshCall(Volunteer.Id);
                    DistanceToCall = assignWindow.AssignedCallDistance;
                }
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Assign Call Error", "Failed to open the call assignment window.");
            }
        }

        /// <summary>
        /// Logs out the current volunteer (closes the window).
        /// </summary>
        private void LogOut()
        {
            try
            {
                Close();
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Logout Error", "An error occurred while trying to log out.");
            }
        }

        /// <summary>
        /// Opens the volunteer update window to allow editing personal details.
        /// </summary>
        /// <summary>
        /// Opens the volunteer update window to allow editing personal details.
        /// </summary>
        private void OpenUpdateWindow()
        {
            try
            {
                new VolunteerWindow(Volunteer.Id.ToString(), isSelfEdit: true, isAdmin: true).Show();
                Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id.ToString());
                OnPropertyChanged(nameof(Volunteer));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Too many managers"))
                {
                    ErrorHandler.ShowWarning(
                        "Too Many Managers",
                        "Only 2 active managers are allowed. Please deactivate one before adding another."
                    );
                }
                else
                {
                    ErrorHandler.ShowError("Update Error", "Unable to open the update window. Please try again.");
                }
            }
        }


        /// <summary>
        /// Opens a window showing the volunteer's call history.
        /// </summary>
        private void OpenHistoryWindow()
        {
            try
            {
                new CallHistoryWindow(Volunteer.Id).Show();
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("History Error", "Unable to open the call history window.");
            }
        }

        /// <summary>
        /// Notifies the UI about property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invokes PropertyChanged for the given property name.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
