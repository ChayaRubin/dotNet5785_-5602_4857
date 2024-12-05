using DalApi; 
using DO;
using System.Collections.Generic;
using System.Linq;
namespace Dal;
//I explained them each by the program page

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        item.RadioCallId = Config.getNextAssignmentId;
        Call copy = item;
        DataSource.Calls.Add(copy);
    }
    public void Delete(int id)//gpt
    {
            Call? callToRemove = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == id);
            if (callToRemove != null) DataSource.Calls.Remove(callToRemove);
            else throw new DalDoesNotExistException($"Call with RadioCallId {id} does not exists.");
    }

    public void DeleteAll()
    {
        if (!DataSource.Calls.Any()) return; 
        DataSource.Calls.Clear();
    }


    //public Call? Read(int id)
    //{
    //    return DataSource.Calls.FirstOrDefault(item => item.RadioCallId == id); //stage 2
    //}

    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        {
            return filter == null
                ? DataSource.Calls
                : DataSource.Calls.Where(filter);
        }
    }
    public void Update(Call item)
    {
        Call? callToRemove = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == item.RadioCallId);
        if (callToRemove != null) DataSource.Calls.Remove(callToRemove);
        else throw new DalDoesNotExistException($"Call with this Id {item.RadioCallId} does not exists.");
        DataSource.Calls.Add(item);
    }
}
