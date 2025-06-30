using BO;
using BlApi;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;

namespace PL
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        static readonly IBl s_bl = Factory.Get();

        private readonly bool isSelfEdit;
        private readonly bool isAdmin;
        public bool CanEditId => ButtonText == "Add";
        public bool CanEditRole => isAdmin;
        public bool CanEditActiveStatus => ButtonText == "Add" || !s_bl.Call.GetOpenCallsForVolunteer(CurrentVolunteer.Id).Any();
        
        private volatile bool _observerWorking = false;
        public bool HasCallInProgress => CurrentCallInProgress != null;

        private string mapLink = string.Empty;
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public ObservableCollection<BO.PositionEnum> Roles { get; } = new ObservableCollection<BO.PositionEnum>((PositionEnum[])Enum.GetValues(typeof(PositionEnum)));
        public ObservableCollection<DistanceType> DistanceTypes { get; } = new ObservableCollection<DistanceType>((DistanceType[])Enum.GetValues(typeof(DistanceType)));

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

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

        private CallInProgress? currentCallInProgress;
        public CallInProgress? CurrentCallInProgress
        {
            get => currentCallInProgress;
            set
            {
                currentCallInProgress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCallInProgress));
            }
        }
        public string MapLink
        {
            get => mapLink;
            set
            {
                mapLink = value;
                OnPropertyChanged();
            }
        }

        public VolunteerWindow(string idNumber, bool isSelfEdit = false, bool isAdmin = false)
        {
            InitializeComponent();
            this.isSelfEdit = isSelfEdit;
            this.isAdmin = isAdmin;
            DataContext = this;

            try
            {
                if (string.IsNullOrEmpty(idNumber))
                {
                    CurrentVolunteer = new BO.Volunteer();
                    ButtonText = "Add";
                }
                else
                {
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(idNumber);
                    ButtonText = "Update";

                    s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, OnVolunteerChanged);
                    MessageBox.Show($"{CurrentVolunteer.Id}");
                    RefreshCall(CurrentVolunteer.Id);

                }

                OnPropertyChanged(nameof(CanEditRole));
                OnPropertyChanged(nameof(CanEditId));
                OnPropertyChanged(nameof(CanEditActiveStatus));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// טוען את הקריאה הנוכחית שמטופלת ע"י המתנדב, כולל חישוב מרחק.
        /// </summary>
        private void RefreshCall(int volunteerId)
        {
            try
            {
                var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId);
                var firstOpenCall = openCalls.FirstOrDefault();

                if (firstOpenCall != null)
                {
                    var call = s_bl.Call.GetCallDetails(firstOpenCall.Id);
                    var assignment = call.Assignments
                                         .FirstOrDefault(a => a.VolunteerId == volunteerId && a.CompletionTime == null);

                    if (call?.Status != CallStatus.Expired && assignment != null)
                    {
                        CurrentCallInProgress = new CallInProgress
                        {
                            CallId = call.Id,
                            CallType = call.Type,
                            Description = call.Description,
                            Address = call.Address,
                            OpeningTime = call.OpenTime,
                            MaxCompletionTime = call.MaxEndTime,
                            AssignmentStartTime = assignment.AssignTime,
                            DistanceFromVolunteer = s_bl.Call.CalculateDistance(CurrentVolunteer.Latitude, CurrentVolunteer.Longitude, call.Latitude, call.Longitude),
                            Status = call.Status
                        };

                        string origin = Uri.EscapeDataString(CurrentVolunteer.CurrentAddress);
                        string destination = Uri.EscapeDataString(call.Address);
                        MapLink = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}";
                    }
                    else
                    {
                        CurrentCallInProgress = null;
                        MapLink = string.Empty;
                    }
                }
                else
                {
                    CurrentCallInProgress = null;
                    MapLink = string.Empty;
                }
            }
            catch (Exception)
            {
                ErrorHandler.ShowError("Call Loading Error", "Could not load available calls. Please try again later.");
                CurrentCallInProgress = null;
                MapLink = string.Empty;
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
                    string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
                {
                    MessageBox.Show("Please fill all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ButtonText == "Add")
                {
                    if (string.IsNullOrWhiteSpace(VolunteerPassword))
                    {
                        MessageBox.Show("Password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (VolunteerPassword.Length < 8)
                    {
                        MessageBox.Show("Password must be at least 8 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    CurrentVolunteer.Password = VolunteerPassword;
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(VolunteerPassword))
                    {
                        if (VolunteerPassword.Length < 8)
                        {
                            MessageBox.Show("Password must be at least 8 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        CurrentVolunteer.Password = VolunteerPassword;
                    }

                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close();

        private void VolunteerPage_Closing(object? sender, CancelEventArgs e)
        {
            if (CurrentVolunteer != null)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, OnVolunteerChanged);
        }

        private void OnVolunteerChanged()
        {
            if (!_observerWorking) 
            {
                _observerWorking = true; 

                _ = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        var updated = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id.ToString());
                        CurrentVolunteer = updated;
                        RefreshCall(CurrentVolunteer.Id);
                        OnPropertyChanged(nameof(CurrentVolunteer));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to reload volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        _observerWorking = false; 
                    }
                });
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch
            {
                MessageBox.Show("Unable to open the map link in your browser.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
