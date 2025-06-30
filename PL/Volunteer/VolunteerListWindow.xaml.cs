using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;
using DO;

namespace PL.Volunteer
{
    public partial class VolunteersListWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl s_bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        private IEnumerable<VolunteerInList> volunteersListView = new List<VolunteerInList>();
        public bool HasActiveCall { get; set; }

        private volatile bool _observerWorking = false;

        /// <summary>
        /// get and set functions
        /// </summary>
        public IEnumerable<VolunteerInList> VolunteersListView
        {
            get => volunteersListView;
            set
            {
                if (volunteersListView != value)
                {
                    volunteersListView = value;
                    OnPropertyChanged(nameof(VolunteersListView));
                }
            }
        }

        private CallTypeEnum selectedCallType = CallTypeEnum.None;

        /// <summary>
        /// get and set functions
        /// </summary>
        public CallTypeEnum SelectedCallType
        {
            get => selectedCallType;
            set
            {
                if (selectedCallType != value)
                {
                    selectedCallType = value;
                    OnPropertyChanged(nameof(SelectedCallType));
                    LoadVolunteers();
                }
            }
        }

        private VolunteerInList? selectedVolunteer;

        /// <summary>
        /// get and set functions
        /// </summary>
        public VolunteerInList? SelectedVolunteer
        {
            get => selectedVolunteer;
            set
            {
                if (selectedVolunteer != value)
                {
                    selectedVolunteer = value;
                    OnPropertyChanged(nameof(SelectedVolunteer));
                }
            }
        }

        public IEnumerable<CallTypeEnum> CallTypes => Enum.GetValues(typeof(CallTypeEnum)).Cast<CallTypeEnum>();

        /// <summary>
        /// fill up with content and subscribe to observers
        /// </summary>
        public VolunteersListWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Subscribe to the observer for automatic list updates
            s_bl.Volunteer.AddObserver(OnVolunteerListChanged);
        }


        /// <summary>
        /// Observer callback method - called when the volunteer list changes
        /// </summary>
        private void OnVolunteerListChanged()
        {
            if (!_observerWorking)
            {
                _observerWorking = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        LoadVolunteers();
                    }
                    finally
                    {
                        _observerWorking = false;
                    }
                });
            }
        }


        /// <summary>
        /// func that loads the volunteers info.
        /// </summary>
        private void LoadVolunteers()
        {
            try
            {
                CallTypeEnum? filter = SelectedCallType == CallTypeEnum.None ? null : SelectedCallType;
                var volunteers = s_bl.Volunteer.GetVolunteersList(null, null, filter);
                VolunteersListView = volunteers.ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError("Load Error", $"Error loading volunteers: {ex.Message}");
            }
        }

        /// <summary>
        /// func that loads the volunteers info - fix
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVolunteers();
        }

        /// <summary>
        /// func that refreshes the info - fix sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SelectedCallType = CallTypeEnum.None;
            LoadVolunteers();
        }

        /// <summary>
        /// func that closes the window and unsubscribes from observers - fix
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from observer before closing
            s_bl.Volunteer.RemoveObserver(OnVolunteerListChanged);
            Close();
        }

        /// <summary>
        /// Handle window closing event to unsubscribe from observers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unsubscribe from observer when window is closing
            s_bl.Volunteer.RemoveObserver(OnVolunteerListChanged);
        }

        /// <summary>
        /// func that opens the volunteer window on double click on a volunteer in the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolunteersListView_MouseDoubleClick(object? _, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var window = new VolunteerWindow(SelectedVolunteer.Id.ToString(), isSelfEdit: false, isAdmin: true);
                window.Show();
            }
        }

        /// <summary>
        /// func that adds a new volunteer by opening the volunteer window on click on the add button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow("", isSelfEdit: false, isAdmin: true).Show();

        }

        /// <summary>
        /// func that deletes a volunteer by opening a confirmation dialog and then deleting the volunteer if confirmed - fix sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteVolunteer_Click(object? _, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement element && element.DataContext is VolunteerInList volunteer)
            {
                bool confirmed = ErrorHandler.ShowYesNo(
                    "Confirm Deletion",
                    $"Are you sure you want to delete volunteer {volunteer.FullName} (ID {volunteer.Id})?"
                );

                if (confirmed)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
                        // No need to call LoadVolunteers() here - the observer will handle it
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// func that handles property changes for data binding.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}