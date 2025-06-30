using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BlApi;
using BO;
using DO;

namespace PL.Call
{
    public partial class SingleCallWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public BO.Call CurrentCall { get; set; }

        public IEnumerable<CallTypeEnum> CallTypes => Enum.GetValues(typeof(CallTypeEnum)) as IEnumerable<CallTypeEnum>;

        // האם מותר לערוך את סוג הקריאה
        public bool CanEditType =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk;

        // האם מותר לערוך את התיאור
        public bool CanEditDescription =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk;

        // האם מותר לערוך את הכתובת
        public bool CanEditAddress =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk;

        // האם מותר לערוך את זמן הסיום
        public bool CanEditMaxEndTime =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk or
            CallStatus.InProgress or CallStatus.InProgressAtRisk;

        public bool CanEditAllFields =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk;

        public bool CanEditMaxTime =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk or
            CallStatus.InProgress or CallStatus.InProgressAtRisk;

        public bool IsReadOnly =>
            CurrentCall.Status is CallStatus.Closed or CallStatus.Expired;

        public bool HasAssignments => CurrentCall.Assignments != null && CurrentCall.Assignments.Count > 0;

        private volatile bool _observerWorking = false;

        public string TreatmentDuration
        {
            get
            {
                var latestAssignment = CurrentCall?.Assignments?.LastOrDefault(a => a.CompletionTime != null && a.AssignTime != null);
                if (latestAssignment == null)
                    return "—";

                var duration = latestAssignment.CompletionTime - latestAssignment.AssignTime;
                return $"{(int)duration?.TotalMinutes} min";
            }
        }

        public DateTime? MaxEndDate
        {
            get => CurrentCall.MaxEndTime?.Date;
            set
            {
                if (value.HasValue && TimeOnly.TryParse(MaxEndTimeOnly, out var time))
                    CurrentCall.MaxEndTime = value.Value.Date + time.ToTimeSpan();
                OnPropertyChanged();
            }
        }

        public string MaxEndTimeOnly
        {
            get => CurrentCall.MaxEndTime?.ToString("HH:mm:ss") ?? "00:00:00";
            set
            {
                if (TimeOnly.TryParse(value, out var time) && MaxEndDate.HasValue)
                    CurrentCall.MaxEndTime = MaxEndDate.Value.Date + time.ToTimeSpan();
                OnPropertyChanged();
            }
        }

        public SingleCallWindow(CallInList selectedCall)
        {
            InitializeComponent();
            CurrentCall = bl.Call.GetCallDetails(selectedCall.Id);
            DataContext = this;

            bl.Call.AddObserver(selectedCall.Id, OnCallUpdated); 
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsReadOnly)
                {
                    MessageBox.Show("Call is closed or expired. No updates allowed.", "Info",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                bl.Call.UpdateCallDetails(CurrentCall);
                MessageBox.Show("Call updated successfully.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating call: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            bl.Call.RemoveObserver(OnCallUpdated);
            Close();
        }

        private void OnCallUpdated()
        {
            if (_observerWorking) return;
            _observerWorking = true;

            try
            {
                var updatedCall = bl.Call.GetCallDetails(CurrentCall.Id);

                _ = Dispatcher.BeginInvoke(() =>
                {
                    CurrentCall = updatedCall;
                    OnPropertyChanged(nameof(CurrentCall));
                    OnPropertyChanged(nameof(CanEditAllFields));
                    OnPropertyChanged(nameof(CanEditMaxTime));
                    OnPropertyChanged(nameof(CanEditType));
                    OnPropertyChanged(nameof(CanEditDescription));
                    OnPropertyChanged(nameof(CanEditAddress));
                    OnPropertyChanged(nameof(CanEditMaxEndTime));
                    OnPropertyChanged(nameof(IsReadOnly));
                    OnPropertyChanged(nameof(HasAssignments));
                    OnPropertyChanged(nameof(TreatmentDuration));
                    _observerWorking = false;
                });
            }
            catch (Exception ex)
            {
                _ = Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("שגיאה בעדכון פרטי הקריאה: " + ex.Message);
                    _observerWorking = false;
                });
            }
        }

    }
}
