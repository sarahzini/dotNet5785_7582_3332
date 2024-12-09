namespace BIApi;
public interface IBl
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAdmin Admin { get; }
}
