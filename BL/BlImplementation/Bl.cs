namespace BlImplementation;
using BlApi;
using System;

internal class Bl : IBl
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();

//    public void AddObserver(Action listObserver) =>
//StudentManager.Observers.AddListObserver(listObserver); //stage 5
//    public void AddObserver(int id, Action observer) =>
//StudentManager.Observers.AddObserver(id, observer); //stage 5
//    public void RemoveObserver(Action listObserver) =>
//StudentManager.Observers.RemoveListObserver(listObserver); //stage 5
//    public void RemoveObserver(int id, Action observer) =>
//StudentManager.Observers.RemoveObserver(id, observer); //stage 5

}
