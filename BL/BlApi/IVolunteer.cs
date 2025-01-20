using BO;
using DO;
using Helpers;
namespace BlApi;

public interface IVolunteer: IObservable
{
    /// <summary>
    /// Alls the Methods that are used, they are implemented in the CallImplementation class
    /// </summary>
    DO.Job Login(int id, string password);
    IEnumerable<BO.VolunteerInList>? GetVolunteersInList(bool? isActive = null, BO.SystemType? filterValue = null, VolunteerInListFieldSort? sortField = null);
    BO.Volunteer GetVolunteerDetails(int volunteerId);
    void UpdateVolunteer(int requesterId, BO.Volunteer volunteer);
    void DeleteVolunteer(int volunteerId);
    void AddVolunteer(BO.Volunteer volunteer);
    string GetName(int volunteerId);

}
