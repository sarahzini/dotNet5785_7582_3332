
using BlApi;
using BO;
using DO;
using Helpers;
using BlImplementation;

namespace BlImplementation;

internal class CallImplementation: ICall
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
    IEnumerable<BO.CallInList>? ICall.GetSortedCallsInList(CallInListField? filterField, object? filterValue, CallInListField? sortField)
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
    void ICall.UpdateCallDetails(BO.Call callUptade)
    {
        try
        {
            CallManager.ValidateCallDetails(callUptade);

            // Update the latitude and longitude based on the validated address
            (callUptade.CallLatitude, callUptade.CallLongitude) = CallManager.GetCoordinatesFromAddress(callUptade.CallAddress);

            // Convert the business object to a data object by calling a method in manager
            DO.Call callUpdate = CallManager.ConvertToDataCall(callUptade);

            // Attempt to update the call in the data layer
            _dal.Call.Update(callUpdate);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("We cannot update this call: ",ex);
        }
    }
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
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"We cannot delete the call {callId}: ", ex);
        }
    }
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
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException($"We cannot delete the call {call.CallId}: ",ex);
        }
    }
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
                if (call.MaxEnd.HasValue && call.MaxEnd < ClockManager.Now)
                {
                    throw new BO.BLInvalidOperationException("The call has expired.");
                }
            }

            // Create a new assignment (the assignment id will be take from the config in create)
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                Begin = ClockManager.Now,
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
}
