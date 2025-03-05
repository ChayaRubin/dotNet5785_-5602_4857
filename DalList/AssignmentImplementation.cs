using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
namespace Dal;
//I explained them each by the program page
internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {

        item.Id = Config.NextAssignmentId;
        Assignment copy = item;
        DataSource.Assignments.Add(copy);
    }
    public void Delete(int id)//gpt
    {
        Assignment? AssignmentToRemove = DataSource.Assignments.FirstOrDefault(a => a?.Id == id);
        if (AssignmentToRemove != null) DataSource.Assignments.Remove(AssignmentToRemove);
        else throw new DalDoesNotExistException($"Assignment with this Id {id} does not exists.");
    }

    public void DeleteAll()
    {
        if (!DataSource.Assignments.Any()) return; 
        DataSource.Assignments.Clear(); 
    }

    public Assignment Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }


    public Assignment? ReadSingle(Func<Assignment, bool> condition)
    {
        return DataSource.Assignments.FirstOrDefault(condition);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
            return filter == null
                ? DataSource.Assignments
                : DataSource.Assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        Assignment? AssignmentToRemove = DataSource.Assignments.FirstOrDefault(c => c?.Id == item.Id);
        if (AssignmentToRemove != null) DataSource.Assignments.Remove(AssignmentToRemove);
        else throw new DalDoesNotExistException($"Assignment with this Id {item.Id} does not exists.");
        DataSource.Assignments.Add(item);
    }

}


