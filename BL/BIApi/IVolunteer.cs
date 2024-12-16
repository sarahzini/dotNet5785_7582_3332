using BO;
using DO;
using Helpers;

namespace BIApi;

public interface IVolunteer
{
    DO.Job Login(string name, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteersInList(bool? isActive = null, VolunteerInListFieldSort? sortField = null);
    BO.Volunteer GetVolunteerDetails(int volunteerId);
    void UpdateVolunteer(int requesterId, BO.Volunteer volunteer);
    void DeleteVolunteer(int volunteerId);
    void AddVolunteer(BO.Volunteer volunteer);

}
