using BlApi;
using BO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PL
{
    public partial class CallHistoryWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();
        private List<ClosedCallInList> allCalls;

        public ObservableCollection<ClosedCallInList> FilteredCalls { get; set; } = new();

        private string? selectedStatusFilter;
        public string? SelectedStatusFilter
        {
            get => selectedStatusFilter;
            set
            {
                selectedStatusFilter = value;
                ApplyFilter();
                OnPropertyChanged();
            }
        }

        public List<string> StatusOptions { get; set; } = new()
        {
            "All",
            "Treated",
            "Canceled",
            "SelfCanceled",
            "Expired"
        };

        private string statistics;
        public string Statistics
        {
            get => statistics;
            set { statistics = value; OnPropertyChanged(); }
        }
        private readonly int volunteerId;

        public CallHistoryWindow(int volunteerId)
        {
            InitializeComponent();
            DataContext = this;

            this.volunteerId = volunteerId;

            allCalls = _bl.Call.GetClosedCallsByVolunteer(volunteerId).OrderByDescending(c => c.EndTreatmentTime).ToList();
            SelectedStatusFilter = "All";

            // 👇 ADD THIS:
            _bl.Call.AddObserver(volunteerId, RefreshHistory);
            Closed += (_, __) => _bl.Call.RemoveObserver(volunteerId, RefreshHistory);
        }

        private void RefreshHistory()
        {
            Dispatcher.Invoke(() =>
            {
                allCalls = _bl.Call.GetClosedCallsByVolunteer(volunteerId)
                                   .OrderByDescending(c => c.EndTreatmentTime)
                                   .ToList();
                ApplyFilter();
            });
        }

        private void ApplyFilter()
        {
            FilteredCalls.Clear();

            var filtered = SelectedStatusFilter switch
            {
                "Treated" => allCalls.Where(c => c.CompletionType == CallStatus.Treated),
                "Canceled" => allCalls.Where(c => c.CompletionType == CallStatus.Canceled),
                "SelfCanceled" => allCalls.Where(c => c.CompletionType == CallStatus.SelfCanceled),
                "Expired" => allCalls.Where(c => c.CompletionType == CallStatus.Expired),
                _ => allCalls
            };

            foreach (var call in filtered)
                FilteredCalls.Add(call);

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            int total = FilteredCalls.Count;
            int treated = FilteredCalls.Count(c => c.CompletionType == CallStatus.Treated);
            int canceled = FilteredCalls.Count(c => c.CompletionType == CallStatus.Canceled || c.CompletionType == CallStatus.SelfCanceled);
            int expired = FilteredCalls.Count(c => c.CompletionType == CallStatus.Expired);

            Statistics = $"Total: {total} | ✅ Treated: {treated} | ❌ Canceled: {canceled} | ⏰ Expired: {expired}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


