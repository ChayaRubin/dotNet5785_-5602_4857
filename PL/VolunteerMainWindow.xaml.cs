using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        public ObservableCollection<BO.Call> VolunteerCalls { get; set; } = new();
        public ICommand UpdateCommand { get; set; }
        public ICommand HistoryCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public VolunteerMainWindow(string volunteerId)
        {
            InitializeComponent();
            try
            {
                Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
                MessageBox.Show($"Loaded Volunteer Id = {Volunteer.Id}");
                RefreshCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load volunteer: " + ex.Message);
                Close();
            }

            UpdateCommand = new RelayCommand(_ => OpenUpdateWindow());
            HistoryCommand = new RelayCommand(_ => OpenHistoryWindow());
            LogoutCommand = new RelayCommand(_ => Close());
            DataContext = this;
        }

        private void RefreshCalls()
        {
            try
            {
                VolunteerCalls.Clear();
                var allCalls = _bl.Call.GetCallList().OfType<CallInList>().ToList();
                MessageBox.Show($"Total Calls in system: {allCalls.Count}");

                foreach (var call in allCalls)
                {
                    if (call.Assignments == null)
                    {
                        MessageBox.Show($"Call {call.Id} has no assignments");
                        continue;
                    }

                    foreach (var assignment in call.Assignments)
                    {
                        MessageBox.Show($"Checking assignment for volunteer {assignment.VolunteerId} in call {call.Id}");

                        if (assignment.VolunteerId == Volunteer.Id)
                        {
                            var fullCall = _bl.Call.GetCallDetails(call.Id);
                            VolunteerCalls.Add(fullCall);
                            MessageBox.Show($"Added call {call.Id} to volunteer's list");
                            break;
                        }
                    }
                }

                MessageBox.Show($"Total matched calls for volunteer {Volunteer.Id}: {VolunteerCalls.Count}");
                OnPropertyChanged(nameof(VolunteerCalls));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during call refresh: " + ex.Message);
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
            MessageBox.Show("History window not implemented.");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
