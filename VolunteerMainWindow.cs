using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL
{
    public partial class VolunteerMainWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();
        public BO.Volunteer Volunteer { get; set; }
        public BO.Call? CurrentCall { get; set; }

        public ICommand UpdateCommand { get; set; }
        public ICommand HistoryCommand { get; set; }
        public ICommand ChooseCallCommand { get; set; }
        public ICommand FinishCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public bool HasCallInProgress => CurrentCall != null;

        public VolunteerMainWindow(string volunteerId)
        {
            InitializeComponent();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
            CurrentCall = _bl.Call.GetCallList()
                .OfType<BO.CallInList>()
                .Where(call => call.Assignments != null && call.Assignments.Any(a => a.VolunteerId.ToString() == volunteerId))
                .Select(call => _bl.Call.GetCallDetails(call.Id))
                .FirstOrDefault(call => call.Status.ToString().Contains("InProgress"));

            UpdateCommand = new RelayCommand(_ => OpenUpdateWindow());
            HistoryCommand = new RelayCommand(_ => OpenHistoryWindow());
            ChooseCallCommand = new RelayCommand(_ => OpenChooseCallWindow(), _ => !HasCallInProgress);
            FinishCommand = new RelayCommand(_ => FinishCall(), _ => HasCallInProgress);
            CancelCommand = new RelayCommand(_ => CancelCall(), _ => HasCallInProgress);

            DataContext = this;
        }

        private void OpenUpdateWindow()
        {
            // new VolunteerUpdateWindow(Volunteer.Id).ShowDialog();
            // Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
            // OnPropertyChanged(nameof(Volunteer));
        }

        private void OpenHistoryWindow()
        {
            // new VolunteerHistoryWindow(Volunteer.Id).Show();
        }

        private void OpenChooseCallWindow()
        {
            // new ChooseCallWindow(Volunteer.Id).Show();
            // RefreshCall();
        }

        private void FinishCall()
        {
            try
            {
                var assignmentId = CurrentCall?.Assignments?.LastOrDefault(a => a.VolunteerId.ToString() == Volunteer.Id)?.Id ?? 0;
                _bl.Call.CloseCall(int.Parse(Volunteer.Id), assignmentId);
                RefreshCall();
                MessageBox.Show("Treatment finished successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to finish call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCall()
        {
            try
            {
                var assignmentId = CurrentCall?.Assignments?.LastOrDefault(a => a.VolunteerId.ToString() == Volunteer.Id)?.Id ?? 0;
                _bl.Call.CancelCall(int.Parse(Volunteer.Id), assignmentId);
                RefreshCall();
                MessageBox.Show("Call cancelled successfully.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to cancel call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshCall()
        {
            CurrentCall = _bl.Call.GetCallList()
                .OfType<BO.CallInList>()
                .Where(call => call.Assignments != null && call.Assignments.Any(a => a.VolunteerId.ToString() == Volunteer.Id))
                .Select(call => _bl.Call.GetCallDetails(call.Id))
                .FirstOrDefault(call => call.Status.ToString().Contains("InProgress"));

            OnPropertyChanged(nameof(CurrentCall));
            OnPropertyChanged(nameof(HasCallInProgress));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}