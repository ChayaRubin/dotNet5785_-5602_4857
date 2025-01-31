namespace BlApi;

public interface IAdmin
{

        // מתודת בקשת שעון
        DateTime GetSystemClock();

        // מתודת קידום שעון
        void AdvanceSystemClock(BO.TimeUnit timeUnit);

        // מתודת בקשת טווח זמן סיכון
        TimeSpan GetRiskTimeRange();

        // מתודת הגדרת טווח זמן סיכון
        void SetRiskTimeRange(TimeSpan riskTimeRange);

        // מתודת איפוס בסיס נתונים
        void ResetDatabase();

        // מתודת אתחול בסיס נתונים
       void InitializeDatabase();
}
