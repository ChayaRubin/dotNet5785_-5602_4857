//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using BlApi;
//using BO;
//using BlImplementation;
//using DO;

//namespace PL.Call
//{
//    public partial class CallsListWindow : Window, INotifyPropertyChanged
//    {
//        private readonly IBl s_bl = Factory.Get();
//        private readonly DalApi.IDal _dal = DalApi.Factory.Get;


//        public event PropertyChangedEventHandler? PropertyChanged;

//        private IEnumerable<CallForDisplay> callsListView = new List<CallForDisplay>();
//        public IEnumerable<CallForDisplay> CallsListView
//        {
//            get => callsListView;
//            set
//            {
//                if (callsListView != value)
//                {
//                    callsListView = value;
//                    OnPropertyChanged(nameof(CallsListView));
//                }
//            }
//        }


//        private CallTypeEnum selectedCallType = CallTypeEnum.None;
//        public CallTypeEnum SelectedCallType
//        {
//            get => selectedCallType;
//            set
//            {
//                if (selectedCallType != value)
//                {
//                    selectedCallType = value;
//                    OnPropertyChanged(nameof(SelectedCallType));
//                    LoadCalls();
//                }
//            }
//        }

//        private CallInList? selectedCall;
//        public CallInList? SelectedCall
//        {
//            get => selectedCall;
//            set
//            {
//                if (selectedCall != value)
//                {
//                    selectedCall = value;
//                    OnPropertyChanged(nameof(SelectedCall));
//                }
//            }
//        }

//        public IEnumerable<CallTypeEnum> CallTypes => Enum.GetValues(typeof(CallTypeEnum)).Cast<CallTypeEnum>();

//        public CallsListWindow()
//        {
//            InitializeComponent();
//            DataContext = this;
//            s_bl.Call.AddObserver(OnCallListChanged);
//            LoadCalls();
//        }

//        private void Window_Loaded(object sender, RoutedEventArgs e) => LoadCalls();

//        private void LoadCalls()
//        {
//            try
//            {
//                var rawList = s_bl.Call.GetCallList().ToList();

//                if (SelectedCallType != CallTypeEnum.None)
//                {
//                    rawList = rawList.Where(c => c.Type == SelectedCallType).ToList();
//                }

//                CallsListView = rawList.Select(call => new CallForDisplay
//                {
//                    OriginalCall = call
//                }).ToList();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }


//        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadCalls();

//        private void BtnClose_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Call.RemoveObserver(OnCallListChanged);
//            Close();
//        }

//        private void Window_Closing(object sender, CancelEventArgs e)
//        {
//            s_bl.Call.RemoveObserver(OnCallListChanged);
//        }

//        private void DeleteCall_Click(object sender, RoutedEventArgs e)
//        {
//            Button deleteButton = e.Source as Button;
//            if (deleteButton == null)
//            {
//                MessageBox.Show("Error: Delete button not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                return;
//            }

//            CallForDisplay callToDelete = deleteButton.DataContext as CallForDisplay;
//            if (callToDelete == null)
//            {
//                MessageBox.Show("Error: Call context not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                return;
//            }

//            try
//            {
//                BO.Call c = s_bl.Call.GetCallDetails(callToDelete.Id);

//                if (c != null)
//                {
//                    s_bl.Call.DeleteCall(c.Id);
//                    MessageBox.Show("Call deleted successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//                else
//                {
//                    MessageBox.Show("Call details not found.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error deleting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }


//        private void CancelAssignment_Click(BO.CallInList call)
//        {
//            if (call?.Assignments?.Any() == true)
//            {
//                var lastAssignment = call.Assignments.Last();

//                if (lastAssignment.VolunteerId is int volunteerId)
//                {
//                    try
//                    {
//                        // Use the actual assignment ID instead of the call ID
//                        Assignment assignment = _dal.Assignment.Read(a => a.CallId == call.Id && a.VolunteerId == volunteerId);
//                        MessageBox.Show(assignment.ToString(), "sdfghj");

//                        if (assignment != null)
//                        {
//                            s_bl.Call.CancelCall(volunteerId, assignment.Id);
//                            LoadCalls();
//                        }
//                        else
//                        {
//                            MessageBox.Show("Assignment not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Error cancelling assignment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                    }
//                }
//                else
//                {
//                    MessageBox.Show("Volunteer ID is missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//            else
//            {
//                MessageBox.Show("No assignment available to cancel.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//        }


//        private bool CanCancelAssignmentExecute(BO.CallInList call)
//        {
//            return call?.Assignments?.Any() == true;
//        }



//        private void OnCallListChanged() => Dispatcher.Invoke(LoadCalls);

//        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

//        private void CallsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {

//        }

//        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
//        {
//    // e.OriginalSource is the element that raised the event
//            if (e.OriginalSource is Button btn)
//            {
//                var dataContext = btn.DataContext;

//                if (dataContext is CallForDisplay callDisplay)
//                    CancelAssignment_Click(callDisplay.OriginalCall);
//                else if (dataContext is CallInList call)
//                    CancelAssignment_Click(call);
//                else
//                    MessageBox.Show("Unable to find call context for cancellation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//            else
//            {
//                MessageBox.Show("Cancel button source not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//    }
//}using BO;using BlApi;using BlApi;using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL.Call
{
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl s_bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        private IEnumerable<CallInList> callsListView = new List<CallInList>();
        public IEnumerable<CallInList> CallsListView
        {
            get => callsListView;
            set
            {
                if (callsListView != value)
                {
                    callsListView = value;
                    OnPropertyChanged(nameof(CallsListView));
                }
            }
        }

        private CallTypeEnum selectedCallType = CallTypeEnum.None;
        public CallTypeEnum SelectedCallType
        {
            get => selectedCallType;
            set
            {
                if (selectedCallType != value)
                {
                    selectedCallType = value;
                    OnPropertyChanged(nameof(SelectedCallType));
                    LoadCalls();
                }
            }
        }

        private CallInList? selectedCall;
        public CallInList? SelectedCall
        {
            get => selectedCall;
            set
            {
                if (selectedCall != value)
                {
                    selectedCall = value;
                    OnPropertyChanged(nameof(SelectedCall));
                }
            }
        }

        public IEnumerable<CallTypeEnum> CallTypes => Enum.GetValues(typeof(CallTypeEnum)).Cast<CallTypeEnum>();

        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;
            s_bl.Call.AddObserver(OnCallListChanged);
        }

        private void OnCallListChanged()
        {
            Dispatcher.Invoke(() =>
            {
                LoadCalls();
            });
        }

        private void LoadCalls()
        {
            try
            {
                CallTypeEnum? filter = SelectedCallType == CallTypeEnum.None ? null : SelectedCallType;
                var calls = s_bl.Call.GetCallList().Where(c => !filter.HasValue || c.Type == filter).ToList();
                CallsListView = calls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCalls();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SelectedCallType = CallTypeEnum.None;
            LoadCalls();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Call.RemoveObserver(OnCallListChanged);
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            s_bl.Call.RemoveObserver(OnCallListChanged);
        }

        private void CallsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (SelectedCall != null)
            //{
            //    var window = new CallWindow(SelectedCall); // assumes you have CallWindow(CallInList call)
            //    window.Show();
            //}
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new CallWindow(); // assumes you have CallWindow() for adding
            window.Show();
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement element && element.DataContext is CallInList call)
            {
                try
                {
                    s_bl.Call.DeleteCall(call.Id);
                    // Observer will refresh the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Cannot delete this call. Make sure it is open and has no assignments.",
                        "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            //if (e.Source is FrameworkElement element && element.DataContext is CallInList call)
            //{
            //    try
            //    {
            //        // Step 1: Get the volunteer ID from the visible assignment
            //        var previewAssignment = call.Assignments?.LastOrDefault(a => a.CompletionTime == null);

            //        if (previewAssignment?.VolunteerId == null)
            //        {
            //            MessageBox.Show("No active assignment to cancel.",
            //                "Cancel Assignment Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            //            return;
            //        }

            //        int volunteerId = previewAssignment.VolunteerId.Value;

            //        // Step 2: Get the full call from BL
            //        var fullCall = s_bl.Call.GetCallDetails(call.Id);

            //        // Step 3: Find matching assignment
            //        var fullAssignment = fullCall.Assignments?.FirstOrDefault(a =>
            //            a.VolunteerId == volunteerId && a.CompletionTime == null);

            //        if (fullAssignment == null)
            //        {
            //            MessageBox.Show("Full assignment not found.",
            //                "Cancel Assignment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            //            return;
            //        }

            //        // Step 4: Cancel it using the full assignment ID
            //        s_bl.Call.CancelCall(volunteerId, fullAssignment.Id);

            //        // Step 5: Success
            //        MessageBox.Show("Assignment was successfully canceled.",
            //            "Cancel Assignment", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //}
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

