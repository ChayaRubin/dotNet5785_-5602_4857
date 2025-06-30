using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
namespace Dal;

//I explained them each by the program page

internal class VolunteerImplementation :IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        
        if (DataSource.Volunteers.Any(c => c?.Id == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with Id {item.Id} already exists.");
       
        DataSource.Volunteers.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer? VolunteerToRemove = DataSource.Volunteers.FirstOrDefault(c => c?.Id == id);
        if (VolunteerToRemove != null) DataSource.Volunteers.Remove(VolunteerToRemove);
        else throw new DalDoesNotExistException($"Volunteer with id {id} does not exists.");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (!DataSource.Volunteers.Any()) return; 
        DataSource.Volunteers.Clear(); 
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null
                ? DataSource.Volunteers
                : DataSource.Volunteers.Where(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        Volunteer? VolunteerToRemove = DataSource.Volunteers.FirstOrDefault(c => c?.Id == item.Id);
        if (VolunteerToRemove != null) DataSource.Volunteers.Remove(VolunteerToRemove);
        else throw new DalDoesNotExistException($"Volunteer with this Id {item.Id} does not exists.");
        DataSource.Volunteers.Add(item);
    }
}
