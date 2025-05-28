using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteersListWindow : Window
    {
        private readonly IBl s_bl = Factory.Get();

        // רשימת מתנדבים
        public IEnumerable<VolunteerInList> VolunteersListView
        {
            get => (IEnumerable<VolunteerInList>)GetValue(VolunteersListViewProperty);
            set => SetValue(VolunteersListViewProperty, value);
        }

        public static readonly DependencyProperty VolunteersListViewProperty =
            DependencyProperty.Register(
                nameof(VolunteersListView),
                typeof(IEnumerable<VolunteerInList>),
                typeof(VolunteersListWindow)
            );

        // המתנדב הנבחר
        public VolunteerInList? SelectedVolunteer { get; set; }

        public VolunteersListWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FilterByComboBox.ItemsSource = Enum.GetValues(typeof(CallTypeEnum));
            FilterByComboBox.SelectedItem = CallTypeEnum.Non_Urgent;
            LoadVolunteers();
        }

        private void FilterByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadVolunteers();
        }

        private void LoadVolunteers()
        {
            try
            {
                var selectedCallType = (CallTypeEnum)FilterByComboBox.SelectedItem;
                CallTypeEnum? callTypeFilter = selectedCallType == CallTypeEnum.Non_Urgent ? null : selectedCallType;
                var volunteers = s_bl.Volunteer.GetVolunteersList(null, null, callTypeFilter);
                VolunteersListView = volunteers.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            FilterByComboBox.SelectedItem = CallTypeEnum.Non_Urgent;
            LoadVolunteers();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VolunteersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VolunteersListViewControl.SelectedItem is VolunteerInList selected)
            {
                string idAsString = selected.Id.ToString();
                var window = new VolunteerWindow(idAsString);
                window.Show();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new VolunteerWindow(); // מצב הוספה אמיתי
            window.Show();
        }

        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is VolunteerInList volunteer)
            {
                var result = MessageBox.Show($"Are you sure you want to delete volunteer {volunteer.FullName} (ID {volunteer.Id})?",
                                             "Confirm Deletion",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
                        LoadVolunteers(); // רענון הרשימה לאחר המחיקה
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
