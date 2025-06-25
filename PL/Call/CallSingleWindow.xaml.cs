using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BlApi;
using BO;

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

        public bool CanEditAllFields =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk;

        public bool CanEditMaxTime =>
            CurrentCall.Status is CallStatus.Open or CallStatus.OpenAtRisk or
            CallStatus.InProgress or CallStatus.InProgressAtRisk;

        public bool IsReadOnly =>
            CurrentCall.Status is CallStatus.Closed or CallStatus.Expired;

        public bool HasAssignments => CurrentCall.Assignments != null && CurrentCall.Assignments.Count > 0;

        public SingleCallWindow(CallInList selectedCall)
        {
            InitializeComponent();
            CurrentCall = bl.Call.GetCallDetails(selectedCall.Id);
            DataContext = this;
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

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
