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
                Type = BO.CallTypeEnum.General_Assistance,
                Description = string.Empty,
                Address = string.Empty,
                Latitude = 0,
                Longitude = 0,
                OpenTime = bl.Admin.GetSystemClock(),
                MaxEndTime = null,
                Status = BO.CallStatus.Open,
                Assignments = new List<BO.CallAssignInList>()
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
                {
                    bl.Call.UpdateCallDetails(CurrentCall);
                }
                else
                {
                    await bl.Call.AddCall(CurrentCall);
                }

                Close();
            }
            catch (ArgumentException argEx)
            {
                ErrorHandler.ShowError("Invalid Input", "Please check the call data");
            }
            catch (TaskCanceledException tcEx)
            {
                ErrorHandler.ShowError("Timeout", "The operation timed out: ");
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError("Save Error", "Failed to save call");
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
