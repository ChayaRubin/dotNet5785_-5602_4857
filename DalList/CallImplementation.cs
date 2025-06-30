using DalApi; 
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
namespace Dal;
//I explained them each by the program page

internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        item.RadioCallId = Config.NextCallId;
        Call copy = item;
        DataSource.Calls.Add(copy);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)//gpt
    {
            Call? callToRemove = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == id);
            if (callToRemove != null) DataSource.Calls.Remove(callToRemove);
            else throw new DalDoesNotExistException($"Call with RadioCallId {id} does not exists.");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (!DataSource.Calls.Any()) return; 
        DataSource.Calls.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        {
            return filter == null
                ? DataSource.Calls
                : DataSource.Calls.Where(filter);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        Call? callToRemove = DataSource.Calls.FirstOrDefault(c => c?.RadioCallId == item.RadioCallId);
        if (callToRemove != null) DataSource.Calls.Remove(callToRemove);
        else throw new DalDoesNotExistException($"Call with this Id {item.RadioCallId} does not exists.");
        DataSource.Calls.Add(item);
    }
}
