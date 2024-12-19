
using BlApi;
using BO;
using DO;
using Helpers;
using System.Xml.Linq;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    DO.Job IVolunteer.Login(string name, string password)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteer => volunteer.Name == name);

        if (volunteer == null)
        {
            throw new BO.BLDoesNotExistException($"Volunteer {name} does not exist in the system !");
        }

        if (volunteer.Password != password)
        {
            throw new BO.BLIncorrectPassword($"Incorrect Password !");
        }
        return volunteer.MyJob;
    }
    IEnumerable<VolunteerInList>? IVolunteer.GetVolunteersInList(bool? isActive, VolunteerInListFieldSort? sortField)
    {
        // Get the full list of volunteers
        IEnumerable<DO.Volunteer>? volunteers = _dal.Volunteer.ReadAll();

        // Filter the list based on the isActive parameter
        if (isActive.HasValue)
        {
            volunteers = volunteers?.Where(v => v.IsActive == isActive.Value);
        }

        // Sort the list based on the sortField parameter
        if (sortField.HasValue)
        {
            volunteers = volunteers?.OrderBy(v => v.GetType().GetProperty(sortField.ToString())?.GetValue(v, null));
        }
        else
        {
            volunteers = volunteers?.OrderBy(v => v.VolunteerId);
        }

        // Convert the list to BO.VolunteerInList and return
        return volunteers?.Select(v => VolunteerManager.ConvertToVolunteerInList(v));
    }
    BO.Volunteer IVolunteer.GetVolunteerDetails(int volunteerId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new BO.BLDoesNotExistException($"Volunteer {volunteerId} does not exist in the system !");
        }

        return VolunteerManager.ConvertToLogicalVolunteer(volunteer, volunteerId);
    } 
    public void UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            if (requesterId != volunteer.VolunteerId || !CallManager.IsRequesterManager(requesterId))
            {
                throw new BLInvalidOperationException("Requester is not authorized to update the volunteer");
            }

            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            DO.Volunteer? oldVolunteer = _dal.Volunteer.Read(volunteer.VolunteerId);
            DO.Volunteer updatedVolunteer = oldVolunteer is null ? throw new BO.BLDoesNotExistException($"Volunteer {volunteer.Name} doesn't exist in the system !")
                :VolunteerManager.ConvertToDataVolunteer(volunteer);

            VolunteerManager.CheckAuthorisationToUpdate(oldVolunteer, updatedVolunteer, CallManager.IsRequesterManager(requesterId));

            // Attempt to update the volunteer in the data layer
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured with {volunteer.Name}: ", ex);
        }
    }
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            IEnumerable<DO.Assignment>? assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

            if (assignments?.Any(a => a.End == null) == true)
            {
                throw new BLInvalidOperationException($"Volunteer {volunteerId} is currently assigned to a call and cannot be deleted");
            }

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured : we cannot delete the volunteer with the ID {volunteerId}. ", ex);
        }
    }
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            // Convert BO.Volunteer to DO.Volunteer
            DO.Volunteer newVolunteer = VolunteerManager.ConvertToDataVolunteer(volunteer);

            // Attempt to add the new volunteer to the data layer
            _dal.Volunteer.Create(newVolunteer);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException($"An error occured : we cannot add the volunteer with the ID {volunteer.VolunteerId}.", ex);
        }
    }
}
