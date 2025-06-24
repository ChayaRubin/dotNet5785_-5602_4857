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

        private void ExecuteLogin(object parameter)
        {
            ErrorMessage = string.Empty;

            if (!int.TryParse(Id, out int userId))
            {
                ErrorMessage = "מספר תעודת זהות חייב להכיל ספרות בלבד.";
                MessageBox.Show(ErrorMessage, "שגיאת קלט", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string role;
            try
            {
                role = _bl.Volunteer.Login(userId, Password);
            }
            catch
            {
                ErrorMessage = "ת.ז. או סיסמה אינם נכונים. אנא נסה שוב.";
                MessageBox.Show(ErrorMessage, "שגיאה בהתחברות", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (role == "Volunteer")
                {
                    var volunteer = _bl.Volunteer.GetVolunteerDetails(Id);
                    new VolunteerMainWindow(Id).Show();
                }
                else if (role == "Manager")
                {
                    new MainWindow().Show();
                }
                else
                {
                    ErrorMessage = "סוג המשתמש אינו מוכר במערכת.";
                    MessageBox.Show(ErrorMessage, "שגיאה בזיהוי משתמש", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _loginWindow.Close();
            }
            catch
            {
                ErrorMessage = "אירעה שגיאה בעת טעינת הנתונים. אנא נסה שוב מאוחר יותר.";
                MessageBox.Show(ErrorMessage, "שגיאת מערכת", MessageBoxButton.OK, MessageBoxImage.Error);
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