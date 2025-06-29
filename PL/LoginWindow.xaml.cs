using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }
    }

    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();
        private readonly Window _loginWindow;

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }

        public LoginViewModel(Window loginWindow)
        {
            _loginWindow = loginWindow;
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private static bool _isManagerLoggedIn = false;

        private void ExecuteLogin(object parameter)
        {
            ErrorMessage = string.Empty;

            if (!int.TryParse(Id, out int userId))
            {
                ErrorMessage = "ID must contain digits only.";
                MessageBox.Show(ErrorMessage, "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string role;
            try
            {
                role = _bl.Volunteer.Login(userId, Password);
            }
            catch
            {
                ErrorMessage = "ID or password is incorrect. Please try again.";
                MessageBox.Show(ErrorMessage, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (role == "Volunteer")
                {
                    var volunteer = _bl.Volunteer.GetVolunteerDetails(Id);
                    new VolunteerMainWindow(Id).Show();
                    return; 
                }
                else if (role == "Manager")
                {
                    if (_isManagerLoggedIn)
                    {
                        ErrorMessage = "A manager is already logged in. Please try again later.";
                        MessageBox.Show(ErrorMessage, "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _isManagerLoggedIn = true;

                    var result = MessageBox.Show(
                        "Do you want to open the Manager Dashboard?",
                        "Choose Screen",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var managerWindow = new MainWindow();
                        managerWindow.Closed += (s, e) => _isManagerLoggedIn = false;
                        managerWindow.Show();
                    }
                    else
                    {
                        var volunteer = _bl.Volunteer.GetVolunteerDetails(Id);
                        var volunteerWindow = new VolunteerMainWindow(Id);
                        volunteerWindow.Closed += (s, e) => _isManagerLoggedIn = false;
                        volunteerWindow.Show();
                    }

                    return; // Do not close the login window
                }
                else
                {
                    ErrorMessage = "Unknown user role.";
                    MessageBox.Show(ErrorMessage, "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch
            {
                ErrorMessage = "An error occurred while loading user data. Please try again later.";
                MessageBox.Show(ErrorMessage, "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object>? _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter!);
        public void Execute(object? parameter) => _execute(parameter!);
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}