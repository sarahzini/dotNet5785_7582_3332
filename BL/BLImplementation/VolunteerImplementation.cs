using BlApi;
using BO;
using DO;
using Helpers;
using System.Xml.Linq;
namespace BlImplementation;
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// This method logs in a volunteer by name and password, and returns the job of the volunteer.
    /// </summary>
    DO.Job IVolunteer.Login(int id, string password)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteer => volunteer.VolunteerId == id);

        if (volunteer == null)
        {
            throw new BO.BLDoesNotExistException($"Volunteer with the id number: {id} does not exist in the system !");
        }

        if (volunteer.Password != password)
        {
            throw new BO.BLIncorrectPassword($"Incorrect Password !");
        }
        return volunteer.MyJob;
    }

    /// <summary>
    /// This method returns a list of volunteers based on the isActive and sortField parameters.
    /// </summary>
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

    /// <summary>
    /// This method returns the details of a volunteer based on the volunteerId parameter.
    /// </summary>
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
                : VolunteerManager.ConvertToDataVolunteer(volunteer);

            VolunteerManager.CheckAuthorisationToUpdate(oldVolunteer, updatedVolunteer, CallManager.IsRequesterManager(requesterId));

            // Attempt to update the volunteer in the data layer
            _dal.Volunteer.Update(updatedVolunteer);
            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.VolunteerId); //stage 5   
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
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
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured : we cannot delete the volunteer with the ID {volunteerId}. ", ex);
        }
    }

    /// <summary>
    /// This method adds a new volunteer to the system it first checks the details by calling the 
    /// ValidateVolunteerDetails method and then converts the BO.Volunteer to DO.Volunteer and adds it to the data layer.
    /// </summary>
    /// <param name="volunteer"></param>
    /// <exception cref="BO.BLAlreadyExistException"></exception>
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
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5   
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException($"An error occured : we cannot add the volunteer with the ID {volunteer.VolunteerId}.", ex);
        }
    }

    /// <summary>
    /// This method adds a list observer to the observers list.
    /// </summary>
    /// <param name="listObserver">The list of observers</param>
    public void AddObserver(Action listObserver) =>
           VolunteerManager.Observers.AddListObserver(listObserver);

    /// <summary>
    /// This method adds an observer to the observers list.
    /// </summary>
    /// <param name="id">The id of the observer</param>
    /// <param name="observer">The observer to add</param>
    public void AddObserver(int id, Action observer) =>
           VolunteerManager.Observers.AddObserver(id, observer);

    /// <summary>
    /// This method removes a list observer from the observers list.
    /// </summary>
    /// <param name="listObserver">The list of observers</param>
    public void RemoveObserver(Action listObserver) =>
           VolunteerManager.Observers.RemoveListObserver(listObserver);

    /// <summary>
    /// This method removes an observer from the observers list.
    /// </summary>
    /// <param name="id">The id of the observer </param>
    /// <param name="observer">The observer to remove </param>
    public void RemoveObserver(int id, Action observer) =>
           VolunteerManager.Observers.RemoveObserver(id, observer);


    /// <summary>
    /// This method returns a list of volunteers filtered by the specified system type.
    /// </summary>
    /// <param name="filterValue">The system type to filter volunteers by. If null, no filtering is applied.</param>
    /// <returns>A list of volunteers filtered by the specified system type.</returns>
    public IEnumerable<BO.VolunteerInList> GetFilteredVolunteersInList(BO.SystemType? filterValue = null)
    {
        // Get the full list of volunteers
        IEnumerable<DO.Volunteer>? volunteers = _dal.Volunteer.ReadAll();

        // Create a list to store the filtered volunteers
        List<DO.Volunteer> filteredVolunteers = new List<DO.Volunteer>();

        // Filter the list based on the filterValue parameter
        if (filterValue.HasValue)
        {
            foreach (var v in volunteers)
            {
                // Get all assignments for the current volunteer
                IEnumerable<DO.Assignment>? assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == v.VolunteerId);

                // Get the call ID of the first assignment that has not ended
                int? actualCallId = assignments?.FirstOrDefault(a => a.End == null)?.CallId;

                // Determine the type of call based on the call ID
                BO.SystemType typeOfCall = actualCallId is null ? BO.SystemType.None :
                    (_dal.Call.Read(c => c.CallId == actualCallId) is null ?
                    BO.SystemType.None : (BO.SystemType)_dal.Call.Read(c => c.CallId == actualCallId).AmbulanceType);

                // Add the volunteer to the filtered list if the type of call matches the filter value
                if (typeOfCall == filterValue)
                    filteredVolunteers.Add(v);
            }
            volunteers = filteredVolunteers;
        }

        // Convert the list to BO.VolunteerInList and return
        return volunteers?.Select(v => VolunteerManager.ConvertToVolunteerInList(v));
    }

    public string GetName(int volunteerId)
    {
        return _dal.Volunteer.Read(volunteer => volunteer.VolunteerId == volunteerId)?.Name;
    }
}

