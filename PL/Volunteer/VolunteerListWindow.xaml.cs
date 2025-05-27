using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BlApi;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteersListWindow : Window
    {
        private readonly IBl s_bl = Factory.Get();

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

        public VolunteersListWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FilterByComboBox.ItemsSource = Enum.GetValues(typeof(CallTypeEnum));
            FilterByComboBox.SelectedItem = CallTypeEnum.None;
            LoadVolunteers();
        }

        private void FilterByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadVolunteers();
        }

        private void LoadVolunteers()
        {
            var selectedCallType = (CallTypeEnum)FilterByComboBox.SelectedItem;
            CallTypeEnum? callTypeFilter = selectedCallType == CallTypeEnum.None ? (CallTypeEnum?)null : selectedCallType;

            var volunteers = s_bl.Volunteer.GetVolunteersList(null, null, callTypeFilter);

            VolunteersListView = volunteers.ToList();

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            FilterByComboBox.SelectedItem = CallTypeEnum.None;
            LoadVolunteers();
        }
    }
}
