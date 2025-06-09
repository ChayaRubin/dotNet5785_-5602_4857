using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BlApi;
using BO;
using System.Threading.Tasks;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        public BO.Call CurrentCall { get; set; }

        public IEnumerable<CallTypeEnum> CallTypes => Enum.GetValues(typeof(CallTypeEnum)) as IEnumerable<CallTypeEnum>;

        public string WindowTitle => isEditMode ? "Edit Call" : "Add Call";
        private readonly bool isEditMode;

        public CallWindow()
        {
            InitializeComponent();
            CurrentCall = new BO.Call
            {
                OpenTime = DateTime.Now,
                MaxEndTime = DateTime.Now.AddHours(1)
            };
            isEditMode = false;
            DataContext = this;
        }

        public CallWindow(CallInList selectedCall)
        {
            InitializeComponent();
            CurrentCall = bl.Call.GetCallDetails(selectedCall.Id);
            isEditMode = true;
            DataContext = this;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isEditMode)
                    bl.Call.UpdateCallDetails(CurrentCall); // sync method
                else
                    await bl.Call.AddCall(CurrentCall);     // async method

                Close(); // auto-close
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
