using BlApi;
using BO;
using DO;
using Helpers;
using BlImplementation;
using static Helpers.CallManager;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Calculates the number of calls for each ambulance type (ICU and Regular)
    /// and returns an array of integers where: 0 for Icu and 1 for Regular
    /// It uses the following process: Fetches all calls from the data layer, Groups the calls
    /// by their `AmbulanceType` using LINQ, Counts the number of calls for each group and
    /// stores the counts in a dictionary and Maps the counts for each type into the result array.
    /// If there are no calls of a specific type, a count of 0 is assigned.
    /// <summary>
    int[] ICall.TypeOfCallCounts()
    {
        IEnumerable<DO.Call>? calls = _dal.Call.ReadAll();

        int[] result = new int[2];

        // Group calls by their SystemType and count the number of calls in each group
        var callCountsByType = calls?
            .GroupBy(call => call.AmbulanceType)
            .Select(group => new { Type = group.Key, Count = group.Count() })
            .ToDictionary(g => g.Type, g => g.Count);

        // Fill the result array with the counts
        if (callCountsByType != null)
        {
            result[(int)DO.SystemType.ICUAmbulance] = callCountsByType.ContainsKey(DO.SystemType.ICUAmbulance) ? callCountsByType[DO.SystemType.ICUAmbulance] : 0;
            result[(int)DO.SystemType.RegularAmbulance] = callCountsByType.ContainsKey(DO.SystemType.RegularAmbulance) ? callCountsByType[DO.SystemType.RegularAmbulance] : 0;
        }

        return result;
    }

    /// <summary>
    /// This method fetches all calls from the data layer, 
    /// filters the calls based on the provided filterField and filterValue then returns the sorted calls.
    /// </summary>
    IEnumerable<BO.CallInList>? ICall.GetSortedCallsInList(CallInListField? filterField=null, object? filterValue=null, CallInListField? sortField=null)
    {
        // Get all calls
        var calls = _dal.Call.ReadAll();
        // Filter calls if filterField and filterValue are provided
        if (filterField != null && filterValue != null)
        {
            var filterProperty = typeof(BO.CallInList).GetProperty(filterField.ToString());
            if (filterProperty != null)
            {
                calls = calls?.Where(call => filterProperty.GetValue(call)?.Equals(filterValue) == true);
            }
        }

        // Sort calls if sortField is provided
        if (sortField != null)
        {
            var sortProperty = typeof(BO.CallInList).GetProperty(sortField.ToString());
            if (sortProperty != null)
            {
                calls = calls?.OrderBy(call => sortProperty.GetValue(call));
            }
        }
        else
        {
            // Default sorting by CallId
            calls = calls?.OrderBy(call => call.CallId);
        }

        // Convert to CallInList and return
        return calls?.Select(call => CallManager.ConvertToCallInList(call)).ToList();
    }

    /// <summary>
    /// This method get the Details of a specific call by its Id using the ConvertToLogicCall method to convert
    /// from DO.Call to BO.Call.
    /// </summary>
    BO.Call ICall.GetCallDetails(int CallId)
    {
        try
        {
            // Get the call details from the data layer
            DO.Call call = _dal.Call.Read(CallId) ?? throw new BO.BLDoesNotExistException($"Call with Id {CallId} does not exist.");

            // Convert the data object to a business object
            return CallManager.ConvertToLogicCall(call);

        }
        catch (BO.BLDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException(ex.Message);
        }
    }

    /// <summary>
    /// This methods updates the details of a specific call by its Id it checks wether the details are valid 
    /// by using the ValidateCallDetails method then updates the latitude and longitude 
    /// based on the validated address by using the GetCoordinatesFromAddress method.
    /// The data is then converted to a data object by calling a method in the CallManager 
    /// using the ConvertToDataCall method and the call is updated in the data layer.
    /// </summary>
    void ICall.UpdateCallDetails(BO.Call callUptade)
    {
        try
        {
            CallManager.ValidateCallDetails(callUptade);

            if (callUptade?.CallLatitude == null && callUptade?.CallLongitude == null)
            {// Update the latitude and longitude based on the validated address
                (callUptade.CallLatitude, callUptade.CallLongitude) = GeocodeService.GetCoordinates(callUptade.CallAddress);
            }

            // Convert the business object to a data object by calling a method in manager
            DO.Call callUpdate = CallManager.ConvertToDataCall(callUptade);

            // Attempt to update the call in the data layer
            _dal.Call.Update(callUpdate);
            CallManager.Observers.NotifyItemUpdated(callUpdate.CallId); //stage 5   
            CallManager.Observers.NotifyListUpdated(); //stage 5   

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("We cannot update this call: ", ex);
        }
    }

    /// <summary>
    /// This method deletes a specific call by its Id it first checks 
    /// if the call can be deleted by checking if the call has been assigned to a volunteer or it is not open. 
    /// If the conditions are met the call is deleted from the data layer.
    /// </summary>
    void ICall.DeleteCall(int callId)
    {
        try
        {
            DO.Call? call = _dal.Call.Read(callId);
            IEnumerable<DO.Assignment>? assignments = _dal.Assignment.ReadAll(assignment => assignment.CallId == callId);

            // Check if the call can be deleted
            if (assignments != null || assignments?.Any(a => a.End != null) == true)
            {
                throw new BO.BLInvalidOperationException("Call cannot be deleted because the call is not open or has been assigned to a volunteer");
            }

            // Attempt to delete the call from the data layer
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5   
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"We cannot delete the call {callId}: ", ex);
        }
    }


    /// <summary>
    /// This method adds a new call to the data layer it first checks if the call details are valid
    /// by using the ValidateCsllDetails method then converts the business object to a data object( BO.call to DO.Call).
    /// Then the call is added to the data layer.
    /// </summary>
    void ICall.AddCall(BO.Call call)
    {
        try
        {
            // Validate call details
            CallManager.ValidateCallDetails(call);

            // Convert BO.call to DO.Call
            DO.Call newCall = CallManager.ConvertToDataCall(call);

            // Attempt to add the new call to the data layer
            _dal.Call.Create(newCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5   
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException($"We cannot delete the call {call.CallId}: ", ex);
        }
    }

    /// <summary>
    /// This method fetches all calls from the data layer,
    /// filters the calls based on the volunteerId and callType parameters,
    /// </summary>
    IEnumerable<ClosedCallInList>? ICall.SortClosedCalls(int volunteerId, BO.SystemType? callType, ClosedCallInListField? sortField)
    {
        // Get the full list of calls with the filter of id
        IEnumerable<DO.Call>? calls = _dal.Call.ReadAll()?
            .Where(call => _dal.Assignment.Read(assignment => assignment.CallId == call.CallId)?.VolunteerId == volunteerId
            && _dal.Assignment.Read(assignment => assignment.CallId == call.CallId)?.MyEndStatus == DO.EndStatus.Completed);

        // Filter the list based on the callType parameter
        if (callType.HasValue)
        {
            calls = calls?.Where(call => call.AmbulanceType == (DO.SystemType)callType);
        }

        // Sort the list based on the sortField parameter
        if (sortField.HasValue)
        {
            calls = calls?.OrderBy(call => call.GetType().GetProperty(sortField.ToString()).GetValue(call, null));
        }
        else
        {
            calls = calls?.OrderBy(call => call.CallId);
        }

        // Convert the list to BO.ClosedCallInList and return
        return calls?.Select(call => CallManager.ConvertToClosedCallInList(call)).ToList();

    }

    /// <summary>
    /// This method fetches all calls from the data layer, 
    /// filters the Open calls based on the volunteerId and callType parameters,
    /// </summary>
    IEnumerable<OpenCallInList>? ICall.SortOpenCalls(int volunteerId, BO.SystemType? callType, OpenCallInListField? sortField)
    {
        // Get the full list of calls with the filter of id
        IEnumerable<DO.Call>? calls = _dal.Call.ReadAll()
            .Where(call => _dal.Assignment.Read(assignment => assignment.CallId == call.CallId)?.VolunteerId == volunteerId
            && _dal.Assignment.Read(assignment => assignment.CallId == call.CallId)?.End == null);

        // Filter the list based on the callType parameter
        if (callType.HasValue)
        {
            calls = calls?.Where(call => call.AmbulanceType == (DO.SystemType)callType);
        }

        // Sort the list based on the sortField parameter
        if (sortField.HasValue)
        {
            calls = calls?.OrderBy(call => call.GetType().GetProperty(sortField.ToString()).GetValue(call, null));
        }
        else
        {
            calls = calls?.OrderBy(call => call.CallId);
        }

        return calls?.Select(call => CallManager.ConvertToOpenCallInList(call, volunteerId)).ToList();
    }

    /// <summary>
    ///This method fetches all calls from the data layer, 
    /// filters the Completed  calls based on the volunteerId and callType parameters,
    /// </summary>
    void ICall.CompleteCall(int volunteerId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);

            // Check if the volunteer is authorized to end the treatment
            if (assignment?.VolunteerId != volunteerId)
            {
                throw new BO.BLInvalidOperationException($"The volunteer with id: {volunteerId} is not authorized to end this treatment because he is not affiliated to this call");
            }

            // Check if the assignment is still open
            if (assignment?.End != null)
            {
                throw new BO.BLInvalidOperationException("The assignment has already been closed.");
            }

            // Update the assignment details using the 'with' expression
            var updatedAssignment = assignment with
            {
                End = _dal.Config.Clock,
                MyEndStatus = DO.EndStatus.Completed,
            };

            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"You cannot update the assignment {assignmentId}: ", ex);
        }
    }

    /// <summary>
    /// This mehod cancels an assignment by its Id it first checks if the requester is authorized to cancel the assignment
    /// by checking if the requester is the volunteer assigned to the assignment or a manager.
    /// </summary>
    void ICall.CancelAssignment(int requesterId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);

            // Check if the requester is authorized to cancel the assignment
            bool isAuthorized = requesterId == assignment?.VolunteerId || CallManager.IsRequesterManager(requesterId);
            if (!isAuthorized)
            {
                throw new BO.BLInvalidOperationException("Requester is not authorized to cancel this assignment.");
            }

            // Check if the assignment is still open
            if (assignment?.End != null)
            {
                throw new BO.BLInvalidOperationException("Cannot cancel an assignment that has already been completed.");
            }

            // Update the assignment details using the 'with' expression
            var updatedAssignment = assignment with
            {
                End = _dal.Config.Clock,
                MyEndStatus = requesterId == assignment.VolunteerId ? DO.EndStatus.SelfCancelled : DO.EndStatus.ManagerCancelled
            };

            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"You cannot update the assignment {assignmentId}: ", ex);
        }
    }

    /// <summary>
    /// Assigns a call to a volunteer by creating a new assignment in the data layer.
    /// </summary>
    void ICall.AssignCallToVolunteer(int volunteerId, int callId)
    {
        try
        {
            // Fetch the call from the data layer
            DO.Call? call = _dal.Call.Read(callId) ?? throw new BO.BLDoesNotExistException($"Call with Id {callId} does not exist.");

            // Check if the call has already been treated or has an open assignment
            IEnumerable<DO.Assignment>? assignments = _dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == callId);
            if (assignments != null)
            {
                if (assignments.Any(a => a.End == null))
                {
                    throw new BO.BLInvalidOperationException("The call is already being treated by another volunteer.");
                }
                if (assignments.Any(a => a.MyEndStatus == DO.EndStatus.Completed))
                {
                    throw new BO.BLInvalidOperationException("The call is already completed.");
                }
                // Check if the call has expired
                if (call.MaxEnd.HasValue && call.MaxEnd < AdminManager.Now)
                {
                    throw new BO.BLInvalidOperationException("The call has expired.");
                }
            }

            // Create a new assignment (the assignment id will be take from the config in create)
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                Begin = AdminManager.Now,
                End = null,
                MyEndStatus = null
            };

            // Attempt to add the new assignment to the data layer
            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException("An error occurred while assigning the call to the volunteer.", ex);
        }
    }

    /// <summary>
    /// This method adds a list observer to the observers list.
    /// </summary>
    /// <param name="listObserver">The list of observers</param>
    public void AddObserver(Action listObserver) =>
           CallManager.Observers.AddListObserver(listObserver);

    /// <summary>
    /// This method adds an observer to the observers list.
    /// </summary>
    /// <param name="id">The id of the observer</param>
    /// <param name="observer">The observer to add</param>
    public void AddObserver(int id, Action observer) =>
           CallManager.Observers.AddObserver(id, observer);

    /// <summary>
    /// This method removes a list observer from the observers list.
    /// </summary>
    /// <param name="listObserver">The list of observers</param>
    public void RemoveObserver(Action listObserver) =>
           CallManager.Observers.RemoveListObserver(listObserver);

    /// <summary>
    /// This method removes an observer from the observers list.
    /// </summary>
    /// <param name="id">The id of the observer </param>
    /// <param name="observer">The observer to remove </param>
    public void RemoveObserver(int id, Action observer) =>
           CallManager.Observers.RemoveObserver(id, observer); 
    

}
