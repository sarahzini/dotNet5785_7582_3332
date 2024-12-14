
using BIApi;
using BO;
using DalApi;
using DO;
using Helpers;

namespace BIImplementation;

internal partial class CallImplementation : ICall
{
    private readonly DalApi.IDal v_dal = DalApi.Factory.Get;
    public IEnumerable<BO.CallInList> GetFilteredAndSortedCalls(CallInListField? filterField, object? filterValue, CallInListField? sortField)
    {
        // Get all calls
        var calls = v_dal.Call.ReadAll();
        // Filter calls if filterField and filterValue are provided
        if (filterField != null && filterValue != null)
        {
            var filterProperty = typeof(BO.CallInList).GetProperty(filterField.ToString());
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
        return calls.Select(call => new BO.CallInList
        {
            AssignId = null, // Assuming AssignId is null for simplicity
            CallId = call.Id,
            TypeOfCall = (BO.SystemType)call.Choice,
            BeginTime = call.DateTime,
            RangeTimeToEnd = call.EndDateTime - call.DateTime,
            NameLastVolunteer = call.Volunteer?.Name,
            ExecutedTime = call.Duration,
            ClosureType = call.Status,
            TotalAssignment = call.Assignments.Count
        });
    } //creer une fct pr convertir call en callinlist en trouvant le assignment qui correspond a call
    public void DeleteCall(int callId)
    {
        try
        {
            // Check if the call exists
            DO.Call call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new BO.BLDoesNotExistException($"Call with ID={callId} does not exist");
            }

            DO.Assignment assignment = AssignmentManager.SearchAssignment(assignment => assignment.CallId == callId);
            // Check if the call can be deleted
            if (assignment.End != null)
            {
                throw new BO.BLInvalidOperationException("Call cannot be deleted because it is not open or has been assigned to a volunteer");
            }

            // Attempt to delete the call from the data layer
            _dal.Call.Delete(callId);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException(ex.Message);
        }
    } //la deuxieme condition 
    public void AddCall(BO.Call call)
    {
        try
        {
            // Validate call details
            CallManager.ValidateCallDetails(call);

            // Convert BO.call to DO.Call
            DO.Call newCall = CallManager.ConvertToLogicCall(call);

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
    }   //ca va pas 
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, DO.SystemType? callType, ClosedCallInListField? sortField)
    {
        try
        {
            // Get the full list of calls with the filter of id 
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll()
                .Where(call => AssignmentManager.SearchAssignment(assignment => assignment.CallId == call.Id).VolunteerId == volunteerId);

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
            return calls.Select(call => new BO.ClosedCallInList
            {
                CallId = call.Id,
                TypeOfCall = call.Choice,
                CallAddress = call.Address,
                BeginTime = call.DateTime,
                BeginActionTime = call.CallAssigns.First(assign => assign.VolunteerId == volunteerId).BeginActionTime,
                EndTime = call.EndDateTime,
                TypeOfEnd = call.ClosureType
            }).ToList();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException(ex.Message);
        }
    }
    public IEnumerable<BO.CallInList> GetOpenCallsForVolunteer(int volunteerId, DO.SystemType? callType, CallInListField? sortField)
    {
        var calls = _dal.Call.ReadAll()
                .Where(call => call.Status == CallStatus.Open || call.Status == CallStatus.OpenRisk);

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

        return sortedCalls.Select(call => new BO.CallInList
        {
            Id = call.Id,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            DateTime = call.DateTime,
            Choice = call.Choice,
            Description = call.Description,
            EndDateTime = call.EndDateTime,
            DistanceFromVolunteer = CalculateDistance(volunteerId, call.Latitude, call.Longitude)
        }).ToList();
    }
}
