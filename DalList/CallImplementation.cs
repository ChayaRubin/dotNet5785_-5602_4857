using DalApi; 
using DO;
using System.Collections.Generic;
namespace Dal;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {

        if (DataSource.Calls.Any(c => c?.RadioCallId == item.RadioCallId))
        {  
            throw new Exception($"Call with Id {item.RadioCallId} already exists.");
        }
        DataSource.Calls.Add(item);
    }
    public void Delete(int id)//gpt
    {
            Call? callToRemove = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == id);
            if (callToRemove != null) DataSource.Calls.Remove(callToRemove);
            else throw new Exception($"Call with RadioCallId {id} does not exists.");
    }

    public void DeleteAll()
    {
        if (!DataSource.Calls.Any()) return; // אם הרשימה ריקה, אין צורך להפעיל כלום
        DataSource.Calls.Clear(); // מסיר את כל האלמנטים ברשימה
    }


    public Call? Read(int id)
    {
        Call? callToRead = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == id);
        return (callToRead);
    }

    public List<Call> ReadAll()
    {
            return new List<Call>(DataSource.Calls); 
    }

    public void Update(Call item)
    {
        Call? callToRemove = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == item.RadioCallId);
        if (callToRemove != null) DataSource.Calls.Remove(callToRemove);
        else throw new Exception($"Call with this Id {item.RadioCallId} does not exists.");
        DataSource.Calls.Add(item);
    }
}
