using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace Dal;
//I explained them each by the program page
internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {

        item.Id = Config.NextAssignmentId;
        Assignment copy = item;
        DataSource.Assignments.Add(copy);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)//gpt
    {
        Assignment? AssignmentToRemove = DataSource.Assignments.FirstOrDefault(a => a?.Id == id);
        if (AssignmentToRemove != null) DataSource.Assignments.Remove(AssignmentToRemove);
        else throw new DalDoesNotExistException($"Assignment with this Id {id} does not exists.");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (!DataSource.Assignments.Any()) return; 
        DataSource.Assignments.Clear(); 
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
            return filter == null
                ? DataSource.Assignments
                : DataSource.Assignments.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        Assignment? AssignmentToRemove = DataSource.Assignments.FirstOrDefault(c => c?.Id == item.Id);
        if (AssignmentToRemove != null) DataSource.Assignments.Remove(AssignmentToRemove);
        else throw new DalDoesNotExistException($"Assignment with this Id {item.Id} does not exists.");
        DataSource.Assignments.Add(item);
    }

}


