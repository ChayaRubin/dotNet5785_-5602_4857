using DalApi;
using DO;
using System.Collections.Generic;
namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {

        if (DataSource.Assignments.Any(a => a?.Id == item.Id))
        {  //*//
            throw new Exception($"Assignment with Id {item.Id} already exists.");
        }
        //item.Id = Config.NextAssignmentId;
        DataSource.Assignments.Add(item);
    }
    public void Delete(int id)//gpt
    {
        Assignment? AssignmentToRemove = DataSource.Assignments.FirstOrDefault(a => a?.Id == id);
        if (AssignmentToRemove != null) DataSource.Assignments.Remove(AssignmentToRemove);
        else throw new Exception($"Assignment with this Id {id} does not exists.");
    }

    public void DeleteAll()
    {
        if (!DataSource.Assignments.Any()) return; // אם הרשימה ריקה, אין צורך להפעיל כלום
        DataSource.Assignments.Clear(); // מסיר את כל האלמנטים ברשימה
    }


    public Assignment? Read(int id)
    {
        Assignment? AssignmentToRead = DataSource.Assignments.FirstOrDefault(a => a?.Id == id);
        return (AssignmentToRead);
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment? AssignmentToRemove = DataSource.Assignments.FirstOrDefault(c => c?.Id == item.Id);
        if (AssignmentToRemove != null) DataSource.Assignments.Remove(AssignmentToRemove);
        else throw new Exception($"Assignment with this Id {item.Id} does not exists.");
        DataSource.Assignments.Add(item);
    }
}


