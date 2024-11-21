using DalApi;
using DO;
using System.Collections.Generic;
namespace Dal;

public class VolunteerImplementation : IVolunteer
{
        public void Create(Volunteer item)
        {

            if (DataSource.Volunteers.Any(c => c?.Id == item.Id)
            {  //*//
            
            }
            DataSource.Volunteers.Add(item);
        }
        public void Delete(int id)//gpt
        {
        Volunteer? VolunteerToRemove = DataSource.Volunteers.FirstOrDefault(c => c?.Id == id);
            if (VolunteerToRemove != null) DataSource.Volunteers.Remove(VolunteerToRemove);
            else throw new Exception($"Call with RadioCallId {id} not found.");
        }

        public void DeleteAll()
        {
            if (!DataSource.Volunteers.Any()) return; // אם הרשימה ריקה, אין צורך להפעיל כלום
            DataSource.Volunteers.Clear(); // מסיר את כל האלמנטים ברשימה
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
            DataSource.Volunteers.Add(item);
        }
}
