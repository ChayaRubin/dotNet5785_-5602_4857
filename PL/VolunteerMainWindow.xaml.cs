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
using DO;
using System.Diagnostics;
using System.Windows.Navigation;
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
            }
        }

        public bool HasCallInProgress => CurrentCall != null;
        public ICommand UpdateCommand { get; set; }
        public ICommand HistoryCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand FinishCommand { get; set; }
        public ICommand AssignCallCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        private string mapUrl;
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

    public bool CanAssignCall => !HasCallInProgress && Volunteer.IsActive;
        public bool CanCancelCall => HasCallInProgress;

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

        private void RefreshCall(int volunteerId)
        {
            try
            {
                // שלב 1: קבל את הקריאות הפתוחות של המתנדב
                var openCalls = _bl.Call.GetOpenCallsForVolunteer(volunteerId);

                // שלב 2: קח את הקריאה הראשונה (אם קיימת)
                var firstOpenCall = openCalls.FirstOrDefault();

                if (firstOpenCall != null)
                {
                    // שלב 3: הבא את הפרטים המלאים
                    var call = _bl.Call.GetCallDetails(firstOpenCall.Id);
                    CurrentCall = call?.Status == CallStatus.Expired ? null : call;
                }
                else
                {
                    CurrentCall = null;
                }

                OnPropertyChanged(nameof(CurrentCall));
                CommandManager.InvalidateRequerySuggested();
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
                OnPropertyChanged(nameof(CurrentCall));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void FinishCurrentCall()
        {
            if (CurrentCall == null)
                return;

            try
            {
                // שימוש בפונקציה מה-BL כדי לקבל את ה-ID של ההקצאה
                int? assignmentId = _bl.Call.GetAssignmentId(CurrentCall.Id, Volunteer.Id);

                if (assignmentId == null)
                {
                    MessageBox.Show("No assignment found for this volunteer.");
                    return;
                }

                _bl.Call.CloseCall(Volunteer.Id, assignmentId.Value);
                MessageBox.Show($"Call {CurrentCall.Id} marked as Treated.");

                CurrentCall = null;
                OnPropertyChanged(nameof(CurrentCall));
                CommandManager.InvalidateRequerySuggested();
                RefreshCall(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to finish call: " + ex.Message);
            }
        }

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

        private void CancelCall()
        {
            try
            {
                int? assignmentId = _bl.Call.GetAssignmentId(CurrentCall.Id, Volunteer.Id);
                if (assignmentId == null)
                {
                    MessageBox.Show("No active assignment found.", "Cancel Error",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _bl.Call.CancelCall(Volunteer.Id, assignmentId.Value);
                MessageBox.Show("Call assignment has been canceled.", "Canceled",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentCall = null;

                RefreshCall(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to cancel call: " + ex.Message,
                                "Cancel Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void OpenUpdateWindow()
        {
            new VolunteerWindow(Volunteer.Id.ToString()).ShowDialog();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id.ToString());
            OnPropertyChanged(nameof(Volunteer));
        }

        private void OpenHistoryWindow()
        {
            new CallHistoryWindow(Volunteer.Id).ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
