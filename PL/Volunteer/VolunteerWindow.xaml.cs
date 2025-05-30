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

//        /// <summary>
//        /// get and set functions
//        /// </summary>
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

//        public string ButtonCallText
//        {
//            get { return (string)GetValue(ButtonCallTextProperty); }
//            set { SetValue(ButtonTextProperty, value); }
//        }

//        public static readonly DependencyProperty ButtonCallTextProperty =
//            DependencyProperty.Register(nameof(ButtonCallText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("See Calls In Progress"));

//        /// <summary>
//        /// constructer for adding a new volunteer
//        /// </summary>
//        public VolunteerWindow()
//        {
//            InitializeComponent();
//            CurrentVolunteer = new BO.Volunteer();
//            ButtonText = "Add";

//        }

//        /// <summary>
//        /// function that receives a selected volunteer from the list for editing
//        /// </summary>
//        /// <param name="selectedVolunteer"></param>
//        public VolunteerWindow(VolunteerInList selectedVolunteer)
//        {
//            InitializeComponent();
//            try
//            {
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(selectedVolunteer.Id.ToString());
//                if (CurrentVolunteer == null)
//                    throw new Exception("Volunteer not found");

//                ButtonText = "Update";
//                ButtonCallText = "See Calls In Progress";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                CurrentVolunteer = new BO.Volunteer();
//                ButtonText = "Add";
//                ButtonCallText = "Assign A Call";
//            }
//        }


//        /// <summary>
//        /// Constructor for loading an existing volunteer by ID number for editing
//        /// </summary>
//        /// <param name="idNumber"></param>
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
//                ButtonCallText = "Assign A Call";
//            }
//        }

//        /// <summary>
//        /// Button click event handler for adding or updating a volunteer.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                if (CurrentVolunteer == null)
//                {
//                    MessageBox.Show("No volunteer data available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }

//                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Id.ToString()) ||
//                    string.IsNullOrWhiteSpace(CurrentVolunteer.FullName) ||
//                    string.IsNullOrWhiteSpace(CurrentVolunteer.Password))
//                {
//                    MessageBox.Show("Please make sure all fields are filled correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                if (CurrentVolunteer.Password.Length < 8)
//                {
//                    MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                if (ButtonText == "Add")
//                {
//                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
//                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                    Close();
//                }
//                else
//                {
//                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
//                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                    Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// function that handles the password change event in the PasswordBox.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
//        {
//            if (CurrentVolunteer != null && sender is PasswordBox passwordBox)
//            {
//                CurrentVolunteer.Password = passwordBox.Password;
//            }
//        }

//        /// <summary>
//        /// class that helps with binding the PasswordBox to a property in the ViewModel.
//        /// </summary>
//        public static class PasswordBoxHelper
//        {
//            public static readonly DependencyProperty BoundPasswordProperty =
//                DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
//                    new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

//            public static string GetBoundPassword(DependencyObject obj) => (string)obj.GetValue(BoundPasswordProperty);
//            public static void SetBoundPassword(DependencyObject obj, string value) => obj.SetValue(BoundPasswordProperty, value);

//            private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//            {
//                if (d is PasswordBox passwordBox)
//                {
//                    string newPassword = (string)e.NewValue;

//                    if (passwordBox.Password != newPassword)
//                        passwordBox.Password = newPassword;

//                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
//                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
//                }
//            }

//            private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
//            {
//                var passwordBox = sender as PasswordBox;
//                if (passwordBox != null)
//                {
//                    SetBoundPassword(passwordBox, passwordBox.Password);
//                }
//            }
//        }

//        /// <summary>
//        /// function that closes the window when the close button is clicked.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void ButtonClose_Click(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }

//        private void VolunteerPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            // Optional: Add logic if needed before window closes
//        }

//        private void ButtonSeeCallinProgress_Click(object sender, RoutedEventArgs e)
//        {

//        }
//    }
//}





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

//        /// <summary>
//        /// get and set functions
//        /// </summary>
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

//        public string ButtonCallText
//        {
//            get { return (string)GetValue(ButtonCallTextProperty); }
//            set { SetValue(ButtonCallTextProperty, value); }
//        }

//        public static readonly DependencyProperty ButtonCallTextProperty =
//            DependencyProperty.Register(nameof(ButtonCallText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("See Calls In Progress"));

//        // Add password property for simple binding
//        public string VolunteerPassword
//        {
//            get { return (string)GetValue(VolunteerPasswordProperty); }
//            set { SetValue(VolunteerPasswordProperty, value); }
//        }

//        public static readonly DependencyProperty VolunteerPasswordProperty =
//            DependencyProperty.Register(nameof(VolunteerPassword), typeof(string), typeof(VolunteerWindow),
//                new PropertyMetadata(string.Empty, OnPasswordChanged));

//        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is VolunteerWindow window && window.CurrentVolunteer != null)
//            {
//                window.CurrentVolunteer.Password = e.NewValue?.ToString() ?? string.Empty;
//            }
//        }

//        /// <summary>
//        /// constructor for adding a new volunteer
//        /// </summary>
//        public VolunteerWindow()
//        {
//            InitializeComponent();
//            CurrentVolunteer = new BO.Volunteer();
//            ButtonText = "Add";
//            DataContext = this;
//        }

//        /// <summary>
//        /// function that receives a selected volunteer from the list for editing
//        /// </summary>
//        /// <param name="selectedVolunteer"></param>
//        public VolunteerWindow(VolunteerInList selectedVolunteer)
//        {
//            InitializeComponent();
//            DataContext = this;

//            try
//            {
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(selectedVolunteer.Id.ToString());
//                if (CurrentVolunteer == null)
//                    throw new Exception("Volunteer not found");

//                VolunteerPassword = CurrentVolunteer.Password ?? string.Empty;
//                ButtonText = "Update";
//                ButtonCallText = "See Calls In Progress";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                CurrentVolunteer = new BO.Volunteer();
//                ButtonText = "Add";
//                ButtonCallText = "Assign A Call";
//            }
//        }

//        /// <summary>
//        /// Constructor for loading an existing volunteer by ID number for editing
//        /// </summary>
//        /// <param name="idNumber"></param>
//        public VolunteerWindow(string idNumber)
//        {
//            InitializeComponent();
//            DataContext = this;

//            try
//            {
//                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(idNumber);
//                if (CurrentVolunteer != null)
//                {
//                    VolunteerPassword = CurrentVolunteer.Password ?? string.Empty;
//                }
//                ButtonText = "Update";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                CurrentVolunteer = new BO.Volunteer();
//                ButtonText = "Add";
//                ButtonCallText = "Assign A Call";
//            }
//        }

//        /// <summary>
//        /// Button click event handler for adding or updating a volunteer.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                if (CurrentVolunteer == null)
//                {
//                    MessageBox.Show("No volunteer data available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }

//                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Id.ToString()) ||
//                    string.IsNullOrWhiteSpace(CurrentVolunteer.FullName) ||
//                    string.IsNullOrWhiteSpace(VolunteerPassword))
//                {
//                    MessageBox.Show("Please make sure all fields are filled correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                if (VolunteerPassword.Length < 8)
//                {
//                    MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }

//                // Ensure password is updated in the volunteer object
//                CurrentVolunteer.Password = VolunteerPassword;

//                if (ButtonText == "Add")
//                {
//                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
//                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                    Close(); // The observer will automatically update the list
//                }
//                else
//                {
//                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
//                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                    Close(); // The observer will automatically update the list
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// function that handles the password change event in the PasswordBox.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
//        {
//            if (sender is PasswordBox passwordBox)
//            {
//                VolunteerPassword = passwordBox.Password;
//            }
//        }

//        /// <summary>
//        /// function that closes the window when the close button is clicked.
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void ButtonClose_Click(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }

//        private void VolunteerPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            // Optional: Add logic if needed before window closes
//        }

//        private void ButtonSeeCallinProgress_Click(object sender, RoutedEventArgs e)
//        {
//            // Implementation for seeing calls in progress
//        }

//        /// <summary>
//        /// Set initial password when PasswordBox loads
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
//        {
//            if (sender is PasswordBox passwordBox)
//            {
//                passwordBox.Password = VolunteerPassword ?? string.Empty;
//            }
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

        /// <summary>
        /// get and set functions
        /// </summary>
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

        public string ButtonCallText
        {
            get { return (string)GetValue(ButtonCallTextProperty); }
            set { SetValue(ButtonCallTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonCallTextProperty =
            DependencyProperty.Register(nameof(ButtonCallText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("See Calls In Progress"));

        // Add password property for simple binding
        public string VolunteerPassword
        {
            get { return (string)GetValue(VolunteerPasswordProperty); }
            set { SetValue(VolunteerPasswordProperty, value); }
        }

        public static readonly DependencyProperty VolunteerPasswordProperty =
            DependencyProperty.Register(nameof(VolunteerPassword), typeof(string), typeof(VolunteerWindow),
                new PropertyMetadata(string.Empty, OnPasswordChanged));

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VolunteerWindow window && window.CurrentVolunteer != null)
            {
                window.CurrentVolunteer.Password = e.NewValue?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// constructor for adding a new volunteer
        /// </summary>
        public VolunteerWindow()
        {
            InitializeComponent();
            CurrentVolunteer = new BO.Volunteer();
            ButtonText = "Add";
            DataContext = this;
        }

        /// <summary>
        /// function that receives a selected volunteer from the list for editing
        /// </summary>
        /// <param name="selectedVolunteer"></param>
        public VolunteerWindow(VolunteerInList selectedVolunteer)
        {
            InitializeComponent();
            DataContext = this;

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(selectedVolunteer.Id.ToString());
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer not found");

                // Don't show the actual password for security - leave it empty
                VolunteerPassword = string.Empty;
                ButtonText = "Update";
                ButtonCallText = "See Calls In Progress";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
                ButtonCallText = "Assign A Call";
            }
        }

        /// <summary>
        /// Constructor for loading an existing volunteer by ID number for editing
        /// </summary>
        /// <param name="idNumber"></param>
        public VolunteerWindow(string idNumber)
        {
            InitializeComponent();
            DataContext = this;

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(idNumber);
                if (CurrentVolunteer != null)
                {
                    // Don't show the actual password for security - leave it empty
                    VolunteerPassword = string.Empty;
                }
                ButtonText = "Update";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
                ButtonCallText = "Assign A Call";
            }
        }

        /// <summary>
        /// Button click event handler for adding or updating a volunteer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
                {
                    MessageBox.Show("Please make sure all required fields are filled correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ButtonText == "Add")
                {
                    // For new volunteers, password is required
                    if (string.IsNullOrWhiteSpace(VolunteerPassword))
                    {
                        MessageBox.Show("Password is required for new volunteers.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (VolunteerPassword.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    CurrentVolunteer.Password = VolunteerPassword;
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // For updates, only update password if a new one was entered
                    if (!string.IsNullOrWhiteSpace(VolunteerPassword))
                    {
                        if (VolunteerPassword.Length < 8)
                        {
                            MessageBox.Show("Password must be at least 8 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        CurrentVolunteer.Password = VolunteerPassword;
                    }
                    // If password field is empty, keep the existing password

                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close(); // The observer will automatically update the list
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// function that handles the password change event in the PasswordBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                VolunteerPassword = passwordBox.Password;
            }
        }

        /// <summary>
        /// function that closes the window when the close button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VolunteerPage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Optional: Add logic if needed before window closes
        }

        private void ButtonSeeCallinProgress_Click(object sender, RoutedEventArgs e)
        {
            // Implementation for seeing calls in progress
        }
    }
}