
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteersListWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl s_bl = Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        private IEnumerable<VolunteerInList> volunteersListView = new List<VolunteerInList>();

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
        /// fill up with content
        /// </summary>
        public VolunteersListWindow()
        {
            InitializeComponent();
            DataContext = this;
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
                MessageBox.Show($"Error loading volunteers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// func that closes the window. - fix
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// func that opens the volunteer window on double click on a volunteer in the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolunteersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is VolunteerInList selected)
            {
                var window = new VolunteerWindow(selected.Id.ToString());
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
            var window = new VolunteerWindow();
            window.Show();
        }

        /// <summary>
        /// func that deletes a volunteer by opening a confirmation dialog and then deleting the volunteer if confirmed - fix sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VolunteerInList volunteer)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete volunteer {volunteer.FullName} (ID {volunteer.Id})?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
                        LoadVolunteers();
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

