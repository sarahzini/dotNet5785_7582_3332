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
        DO.Volunteer? volunteer;

        lock (AdminManager.BlMutex)
            volunteer = _dal.Volunteer.Read(volunteer => volunteer.VolunteerId == id);

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
    public IEnumerable<BO.VolunteerInList> GetVolunteersInList(bool? isActive = null, BO.SystemType? filterValue = null, VolunteerInListFieldSort? sortField = null)
    {
        // Get the full list of volunteers
        IEnumerable<DO.Volunteer>? volunteers;
        lock (AdminManager.BlMutex)
            volunteers = _dal.Volunteer.ReadAll();

        // Filter by isActive if specified
        if (isActive.HasValue)
        {
            volunteers = volunteers?.Where(v => v.IsActive == isActive.Value);
        }

        // Filter by SystemType if specified
        if (filterValue.HasValue && filterValue != BO.SystemType.All)
        {
            var filteredVolunteers = new List<DO.Volunteer>();

            foreach (var v in volunteers ?? Enumerable.Empty<DO.Volunteer>())
            {

                IEnumerable<DO.Assignment>? assignments;

                // Get current assignment for the volunteer
                lock (AdminManager.BlMutex)
                    assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == v.VolunteerId);
                var currentAssignment = assignments?.FirstOrDefault(a => a.End == null);

                if (currentAssignment != null)
                {
                    DO.Call? call;

                    // Get the call type
                    lock (AdminManager.BlMutex)
                        call = _dal.Call.Read(c => c.CallId == currentAssignment.CallId);
                    if (call != null && (BO.SystemType)call.AmbulanceType == filterValue)
                    {
                        filteredVolunteers.Add(v);
                    }
                }
                else if (filterValue == BO.SystemType.None)
                {
                    // If no current assignment and filter is None, include the volunteer
                    filteredVolunteers.Add(v);
                }
            }

            volunteers = filteredVolunteers;
        }

        // Sort the list if specified
        if (sortField.HasValue)
        {
            volunteers = volunteers?.OrderBy(v => v.GetType().GetProperty(sortField.ToString()!)?.GetValue(v, null));
        }
        else
        {
            volunteers = volunteers?.OrderBy(v => v.VolunteerId);
        }

        // Convert to BO.VolunteerInList and return
        return volunteers?.Select(v => VolunteerManager.ConvertToVolunteerInList(v)) ?? Enumerable.Empty<BO.VolunteerInList>();
    }

    /// <summary>
    /// This method returns the details of a volunteer based on the volunteerId parameter.
    /// </summary>
    BO.Volunteer IVolunteer.GetVolunteerDetails(int volunteerId)
    {
        DO.Volunteer? volunteer;

        lock (AdminManager.BlMutex)
            volunteer = _dal.Volunteer.Read(volunteerId);

        if (volunteer == null)
        {
            throw new BO.BLDoesNotExistException($"Volunteer {volunteerId} does not exist in the system !");
        }

        return VolunteerManager.ConvertToLogicalVolunteer(volunteer, volunteerId);
    }
    public void UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            if (requesterId != volunteer.VolunteerId && !CallManager.IsRequesterManager(requesterId))
            {
                throw new BLInvalidOperationException("Requester is not authorized to update the volunteer");
            }

            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            DO.Volunteer? oldVolunteer;
            lock (AdminManager.BlMutex)
                oldVolunteer = _dal.Volunteer.Read(volunteer.VolunteerId);

            DO.Volunteer updatedVolunteer = oldVolunteer is null ? throw new BO.BLDoesNotExistException($"Volunteer {volunteer.Name} doesn't exist in the system !")
                : VolunteerManager.ConvertToDataVolunteer(volunteer);

            VolunteerManager.CheckAuthorisationToUpdate(oldVolunteer, updatedVolunteer, CallManager.IsRequesterManager(requesterId));

            // Attempt to update the volunteer in the data layer
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(updatedVolunteer);
            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.VolunteerId); //stage 5   
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

            if (oldVolunteer.Address != updatedVolunteer.Address)
            {
                //compute the coordinates asynchronously without waiting for the results
                _ = GeocodingService.updateCoordinatesForVolunteerAddressAsync(updatedVolunteer); //stage 7
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured with {volunteer.Name}: ", ex);
        }
    }
    public void DeleteVolunteer(int volunteerId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            IEnumerable<DO.Assignment>? assignments;
            lock (AdminManager.BlMutex)
                assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

            if (assignments?.Any(a => a.End == null) == true)
            {
                throw new BLInvalidOperationException($"Volunteer {volunteerId} is currently assigned to a call and cannot be deleted");
            }
            if(assignments?.Any()==true)
            {
                throw new BLInvalidOperationException($"Volunteer {volunteerId} was already assigned to a call and cannot be deleted ! !");
            }

            lock (AdminManager.BlMutex)
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
    /// <summary>
    /// This method adds a new volunteer to the system it first checks the details by calling the 
    /// ValidateVolunteerDetails method and then converts the BO.Volunteer to DO.Volunteer and adds it to the data layer.
    /// </summary>
    /// <param name="volunteer"></param>
    /// <exception cref="BO.BLAlreadyExistException"></exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            (volunteer.VolunteerLatitude, volunteer.VolunteerLongitude) = GeocodingService.GetCoordinatesS(volunteer.VolunteerAddress);

            // Convert BO.Volunteer to DO.Volunteer
            DO.Volunteer newVolunteer = VolunteerManager.ConvertToDataVolunteer(volunteer);

            // Attempt to add the new volunteer to the data layer
            lock (AdminManager.BlMutex)
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

    public string GetName(int volunteerId)
    {
        lock (AdminManager.BlMutex)
            return _dal.Volunteer.Read(volunteer => volunteer.VolunteerId == volunteerId)!.Name;
    }
}