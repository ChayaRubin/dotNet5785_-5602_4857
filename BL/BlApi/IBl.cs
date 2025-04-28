namespace BlApi;
public interface IBl : IObservable //stage 5 
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAdmin Admin { get; }
}

