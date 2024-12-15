
using BIApi;
using Helpers;

namespace BLImplementation;

internal class CallImplementation: ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public int[] TypeOfCallCounts()
    {
            // Get all calls from the data layer
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();

            // Group calls by their status and count the number of calls in each group
            var callCountsByStatus = calls
                .GroupBy(call => (int)call.Choice)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToDictionary(g => g.Status, g => g.Count);

            // Find the maximum status value to determine the size of the array
            int maxStatus = callCountsByStatus.Keys.Max();
            int[] result = new int[maxStatus+1];

            // Initialize the result array with the size of maxStatus + 1
            //int[] result = new int[3];

            // Fill the result array with the counts
            foreach (var kvp in callCountsByStatus)
            {
                result[kvp.Key] = kvp.Value;
            }

            return result;
    }
    public IEnumerable<BO.CallInList> SortCalls(BO.CallInListField? filterField, object? filterValue, BO.CallInListField? sortField)
    {
        // Get all calls
        var calls = _dal.Call.ReadAll();
        // Filter calls if filterField and filterValue are provided
        if (filterField != null && filterValue != null)
        {
            var filterProperty = typeof(BO.CallInList).GetProperty(filterField!.ToString());
            if (filterProperty != null)
            {
                calls = calls.Where(call => filterProperty.GetValue(call)?.Equals(filterValue) == true);
            }
        }

        // Sort calls if sortField is provided
        if (sortField != null)
        {
            var sortProperty = typeof(BO.CallInList).GetProperty(sortField.ToString());
            if (sortProperty != null)
            {
                calls = calls.OrderBy(call => sortProperty.GetValue(call));
            }
        }
        else
        {
            // Default sorting by CallId
            calls = calls.OrderBy(call => call.Id);
        }

        // Convert to CallInList and return
        return calls.Select(call => CallManager.ConvertToCallInList(call)).ToList(); 
    }
    public BO.Call GetCallDetails(int CallId)
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
    public void UpdateCallDetails(BO.Call callUptade)
    {
        try
        {
            CallManager.ValidateCallDetails(callUptade);

            // Update the latitude and longitude based on the validated address
            (callUptade.CallLatitude, callUptade.CallLongitude) = CallManager.GetCoordinatesFromAddress(callUptade.CallAddress);

            // Convert the business object to a data object by calling a method in manager
            DO.Call callUpdate=CallManager.ConvertToDataCall(callUptade);

            // Attempt to update the call in the data layer
            _dal.Call.Update(callUpdate);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException(ex.Message);
        }
        catch (BO.BLFormatException ex)
        {
            throw new BO.BLFormatException(ex.Message);
        }

    }
    public void CompleteCall(int volunteerId, int assignmentId)
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
            if (assignment.End != null)
            {
                throw new BO.BLInvalidOperationException("The assignment has already been closed.");
            }

            // Update the assignment details using the 'with' expression
            var updatedAssignment = assignment with
            {
                End = AdminManager.Now,
                MyEndStatus = DO.EndStatus.Completed,
            };

            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("kuku je suis la",ex);
        }
      
    }
    public void CancelAssignment(int requesterId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);

            // Check if the requester is authorized to cancel the assignment
            bool isAuthorized = requesterId == assignment?.VolunteerId || CallManager.IsRequesterDirector(requesterId); 
            if (!isAuthorized)
            {
                throw new BO.BLInvalidOperationException("Requester is not authorized to cancel this assignment.");
            }

            // Check if the assignment is still open
            if (assignment?.End != null)
            {
                throw new BO.BLAlreadyCompleted("Cannot cancel an assignment that has already been completed.");
            }

            // Update the assignment details using the 'with' expression
            var updatedAssignment = assignment with
            {
                End = AdminManager.Now,
                MyEndStatus = requesterId == assignment.VolunteerId ? DO.EndStatus.SelfCancelled : DO.EndStatus.DirectorCancelled
            };

            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException(ex.Message);
        }
        catch (BO.BLInvalidOperationException ex)
        {
            throw new BO.BLInvalidOperationException(ex.Message);
        }
        catch (BO.BLAlreadyCompleted ex)
        {
            throw new BO.BLAlreadyCompleted(ex.Message);
        }
    }
    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        try
        {
            // Fetch the call from the data layer
            DO.Call? call = _dal.Call.Read(callId) ?? throw new BO.BLDoesNotExistException($"Call with Id {callId} does not exist.");

            // Check if the call has already been treated or has an open assignment
            var existingAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
            if (existingAssignments.Any(a => a.End == null))
            {
                throw new BO.BLInvalidOperationException("The call is already being treated by another volunteer.");
            }

            // Check if the call has expired
            if (call.EndDateTime.HasValue && call.EndDateTime.Value < DateTime.Now)
            {
                throw new BO.BLInvalidOperationException("The call has expired.");
            }

            // Create a new assignment verifier !!!!!!!!!
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                Begin = DateTime.Now,
                End = null,
                //MyEndStatus = null verifier ca avec Sarah
            };

            // Attempt to add the new assignment to the data layer
            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DALException ex)
        {
            Console.WriteLine(ex.Message);
            throw new BO.BLException("An error occurred while assigning the call to the volunteer.", ex);
        }
    }
    public void DeleteCall(int callId)
    {
        try
        {
            // Check if the call exists
            DO.Call? call = _dal.Call.Read(callId);

            DO.Assignment? assignment = AssignmentManager.SearchAssignment(assignment => assignment.CallId == callId);
            // Check if the call can be deleted
            if (assignment != null||assignment?.End!=null)
            {
                throw new BO.BLInvalidOperationException("Call cannot be deleted because it is not open or has been assigned to a volunteer");
            }

            // Attempt to delete the call from the data layer
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("bla bla",ex);
        }
    } 
    public void AddCall(BO.Call call)
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
            throw new BO.BLAlreadyExistException(ex.Message);
        }
        catch (BO.BLFormatException ex)
        {
            throw new BO.BLFormatException(ex.Message);
        }
    }   
    public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int volunteerId, DO.SystemType? callType, BO.ClosedCallInListField? sortField)
    {
            // Get the full list of calls with the filter of id 
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll()
                .Where(call => AssignmentManager.SearchAssignment(assignment => assignment.CallId == call.Id).VolunteerId == volunteerId
                && AssignmentManager.SearchAssignment(assignment => assignment.CallId == call.Id).MyEndStatus == DO.EndStatus.Completed);

            // Filter the list based on the callType parameter
            if (callType.HasValue)
            {
                calls = calls.Where(call => call.Choice == callType);
            }

            // Sort the list based on the sortField parameter
            if (sortField.HasValue)
            {
                calls = calls.OrderBy(call => call.GetType().GetProperty(sortField.ToString()).GetValue(call, null));
            }
            else
            {
                calls = calls.OrderBy(call => call.Id);
            }

        // Convert the list to BO.ClosedCallInList and return
        return calls.Select(call => CallManager.ConvertToClosedCallInList(call)).ToList();
        
    }
    public IEnumerable<BO.OpenCallInList> SortOpenCalls(int volunteerId, DO.SystemType? callType, BO.CallInListField? sortField)
    {
        // Get the full list of calls with the filter of id 
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll()
            .Where(call => AssignmentManager.SearchAssignment(assignment => assignment.CallId == call.Id).VolunteerId == volunteerId
            && AssignmentManager.SearchAssignment(assignment => assignment.CallId == call.Id).End == null);

        // Filter the list based on the callType parameter
        if (callType.HasValue)
        {
            calls = calls.Where(call => call.Choice == callType);
        }

        // Sort the list based on the sortField parameter
        if (sortField.HasValue)
        {
            calls = calls.OrderBy(call => call.GetType().GetProperty(sortField.ToString()).GetValue(call, null));
        }
        else
        {
            calls = calls.OrderBy(call => call.Id);
        }

        return calls.Select(call => CallManager.ConvertToOpenCallInList(call, volunteerId) ).ToList();
    }
}
