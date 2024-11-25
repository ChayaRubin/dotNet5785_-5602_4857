using DalApi;
using DO;
using System.Collections.Generic;
namespace Dal;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        
        if (DataSource.Volunteers.Any(c => c?.Id == item.Id))
            throw new Exception($"Volunteer with Id {item.Id} already exists.");
       
        DataSource.Volunteers.Add(item);
    }
    public void Delete(int id)//gpt
    {
        Volunteer? VolunteerToRemove = DataSource.Volunteers.FirstOrDefault(c => c?.Id == id);
        if (VolunteerToRemove != null) DataSource.Volunteers.Remove(VolunteerToRemove);
        else throw new Exception($"Volunteer with id {id} does not exists.");
    }

    public void DeleteAll()
    {
        if (!DataSource.Volunteers.Any()) return; 
        DataSource.Volunteers.Clear(); 
    }


    public Volunteer? Read(int id)
    {
        Volunteer? VolunteerToRead = DataSource.Volunteers.FirstOrDefault(c => c?.Id == id);
        return (VolunteerToRead);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        Volunteer? VolunteerToRemove = DataSource.Volunteers.FirstOrDefault(c => c?.Id == item.Id);
        if (VolunteerToRemove != null) DataSource.Volunteers.Remove(VolunteerToRemove);
        else throw new Exception($"Volunteer with this Id {item.Id} does not exists.");
        DataSource.Volunteers.Add(item);
    }
}
