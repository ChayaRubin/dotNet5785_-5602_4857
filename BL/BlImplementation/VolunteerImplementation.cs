using BlApi;
using BO;
using DalApi;
using Helpers;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<CallInList> GetCallsList(
            CallField? filterField,
            object? filterValue,
            CallField? sortField)
    {
        // רשימת קריאות (מניחים שמגיעה ממקור נתונים)
        var calls = GetAllCallsFromDataSource();

        // סינון הקריאות לפי השדה והערך שניתנו
        if (filterField.HasValue && filterValue != null)
        {
            calls = calls.Where(call =>
                GetFieldValue(call, filterField.Value)?.Equals(filterValue) == true);
        }

        // מיון הקריאות לפי השדה שניתן
        if (sortField.HasValue)
        {
            calls = calls.OrderBy(call =>
                GetFieldValue(call, sortField.Value));
        }
        else
        {
            // ברירת מחדל: מיון לפי מזהה הקריאה
            calls = calls.OrderBy(call => call.Id);
        }

        return calls;
    }

    // שליפת הערך של השדה
    private object? GetFieldValue(CallInList call, CallField field)
    {
        return field switch
        {
            CallField.Id => call.Id,
            CallField.Type => call.Type,
            CallField.Description => call.Description,
            CallField.Address => call.Address,
            CallField.Latitude => call.Latitude,
            CallField.Longitude => call.Longitude,
            CallField.OpenTime => call.OpenTime,
            CallField.MaxEndTime => call.MaxEndTime,
            CallField.Status => call.Status,
            _ => null
        };
    }

    // פונקציה לדוגמה לשליפת כל הקריאות ממקור הנתונים
    private IEnumerable<CallInList> GetAllCallsFromDataSource()
    {
        // כאן תוכל לממש שליפה ממסד נתונים
        return new List<CallInList>();
    }





}

