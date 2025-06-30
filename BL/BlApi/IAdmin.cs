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

    // מוסיפה מאזין
    void AddConfigObserver(Action configObserver);

    // מסירה מאזין שכבר לא צריך להאזין לשינויים בקונפיגורציה
    void RemoveConfigObserver(Action configObserver);

    // מוסיפה מאזין שפועל בכל עדכון של השעון
    void AddClockObserver(Action clockObserver);

    // מסירה מאזין שכבר לא צריך להאזין לעדכוני השעון
    void RemoveClockObserver(Action clockObserver);
    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7

}
