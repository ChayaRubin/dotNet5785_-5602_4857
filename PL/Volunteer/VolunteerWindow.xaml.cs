//using BO;
//using BlApi;
//using System;
//using System.Collections.ObjectModel;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL
//{
//    public partial class VolunteerWindow : Window
//    {
//        static readonly IBl s_bl = Factory.Get();

//        public BO.Volunteer CurrentVolunteer
//        {
//            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
//            set { SetValue(CurrentVolunteerProperty, value); }
//        }

//        public static readonly DependencyProperty CurrentVolunteerProperty =
//            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

//        public ObservableCollection<PositionEnum> Roles { get; } = new ObservableCollection<PositionEnum>((PositionEnum[])Enum.GetValues(typeof(PositionEnum)));
//        public ObservableCollection<DistanceType> DistanceTypes { get; } = new ObservableCollection<DistanceType>((DistanceType[])Enum.GetValues(typeof(DistanceType)));

//        public string ButtonText
//        {
//            get { return (string)GetValue(ButtonTextProperty); }
//            set { SetValue(ButtonTextProperty, value); }
//        }

//        public static readonly DependencyProperty ButtonTextProperty =
//            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

//        public VolunteerWindow(VolunteerInList selectedVolunteer)
//        {
//            InitializeComponent();
//            CurrentVolunteer = new BO.Volunteer();
//            ButtonText = "Add";
//        }

//        public VolunteerWindow(string idNumber)
//        {
//            InitializeComponent();
//            try
//            {
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(idNumber);
//                ButtonText = "Update";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                CurrentVolunteer = new BO.Volunteer();
//                ButtonText = "Add";
//            }
//        }

//        private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                // Validate before add/update
//                if (string.IsNullOrWhiteSpace(CurrentVolunteer.FullName) ||
//                    string.IsNullOrWhiteSpace(CurrentVolunteer.Password) ||
//                    CurrentVolunteer.Id <= 0)
//                {
//                    MessageBox.Show("Please make sure all fields are filled correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                var hasMinLength = !string.IsNullOrEmpty(CurrentVolunteer.Password) && CurrentVolunteer.Password.Length >= 8;
//                if (!hasMinLength)
//                {
//                    MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                if (ButtonText == "Add")
//                {
//                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
//                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                    this.Close();
//                }
//                else
//                {
//                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
//                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                    this.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
//        {
//            if (CurrentVolunteer != null && sender is PasswordBox passwordBox)
//            {
//                CurrentVolunteer.Password = passwordBox.Password;
//            }
//        }

//        private void ButtonClose_Click(object sender, RoutedEventArgs e)
//        {
//            this.Close();
//        }

//        private void VolunteerPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            // Optional logic before closing the window
//        }
//    }
//}


using BO;
using BlApi;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public partial class VolunteerWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public ObservableCollection<PositionEnum> Roles { get; } = new ObservableCollection<PositionEnum>((PositionEnum[])Enum.GetValues(typeof(PositionEnum)));
        public ObservableCollection<DistanceType> DistanceTypes { get; } = new ObservableCollection<DistanceType>((DistanceType[])Enum.GetValues(typeof(DistanceType)));

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

        // קונסטרקטור חדש ללא פרמטר למצב הוספה
        public VolunteerWindow()
        {
            InitializeComponent();
            CurrentVolunteer = new BO.Volunteer();
            ButtonText = "Add";
        }

        // קונסטרקטור שמקבל VolunteerInList - טוען פרטים לפי ה-ID שלו
        public VolunteerWindow(VolunteerInList selectedVolunteer)
        {
            InitializeComponent();
            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(selectedVolunteer.Id.ToString());
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer not found");

                ButtonText = "Update";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
            }
        }

        // קונסטרקטור שמקבל מחרוזת idNumber לעריכה
        public VolunteerWindow(string idNumber)
        {
            InitializeComponent();
            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(idNumber);
                ButtonText = "Update";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
            }
        }

        private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer == null)
                {
                    MessageBox.Show("No volunteer data available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Id.ToString()) ||
                    string.IsNullOrWhiteSpace(CurrentVolunteer.FullName) ||
                    string.IsNullOrWhiteSpace(CurrentVolunteer.Password))
                {
                    MessageBox.Show("Please make sure all fields are filled correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CurrentVolunteer.Password.Length < 8)
                {
                    MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer != null && sender is PasswordBox passwordBox)
            {
                CurrentVolunteer.Password = passwordBox.Password;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
