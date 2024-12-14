
using BIApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.IO;

namespace BIImplementation;

internal partial class CallImplementation: ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// This method calculates the number of calls grouped by their statuses, using LINQ's GroupBy method.
    /// </summary>
    /// <returns>The numbers of calls</returns>
    /// <exception cref="BO.BLException">if the count could not be processed for any reasons throw exception</exception>

    public int[] GetCallCounts()
    {
        try
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

            // Initialize the result array with the size of maxStatus + 1
            int[] result = new int[maxStatus + 1];

            // Fill the result array with the counts
            foreach (var kvp in callCountsByStatus)
            {
                result[kvp.Key] = kvp.Value;
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new BO.BLException("An error occurred while getting call counts by status.", ex);
        }
    }

    /// <summary>
    /// This method gets the detail of a call
    /// </summary>
    /// <param name="CallId">Represents the id of a call</param>
    /// <returns>The details of a call</returns>
    /// <exception cref="BO.BLDoesNotExistException">If the call with this Id does not exist throw an exception  </exception>
    /// <exception cref="BO.BLException">If the process of getting the details could not be processed for any reasons throw exception</exception>
    public BO.Call GetCallDetails(int CallId)
    {
        try
        {
            // Get the call details from the data layer
            DO.Call? call = _dal.Call.Read(CallId);
            if (call == null)
            {
                throw new BO.BLDoesNotExistException($"Call with Id {CallId} does not exist.");
            }

            // Get the call assignments from the data layer
            IEnumerable<DO.Call> CallAssigns = _dal.Call.ReadAll(assign => assign.Id == CallId);

            // Convert the data objects to business objects
            List<BO.CallAssignInList> CallAssignInList = CallAssigns
                .Select(assign => new BO.CallAssignInList
                {
                    ///j'arrive passssssssssssssssssssssssss
                    VolunteerId = assign.VolunteerId,
                    VolunteerName = assign.VolunteerName,
                    BeginActionTime = assign.BeginActionTime,
                    EndTime = assign.EndTime,
                    ClosureType = assign.ClosureType
                })
                .ToList();

            // Convert the data object to a business object
            BO.Call BoCall = CallManager.ConvertToLogicCall(call);
            BoCall.CallAssigns = CallAssignInList;

            return BoCall;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new BO.BLException("An error occurred while getting call details.", ex);
        }
    }


    /// <summary>
    /// This method updates the details of a call.
    /// </summary>
    /// <param name="CallUptade">The call to uptade</param>
    /// <exception cref="BO.BLException">If for any reasons the call could not have been been uptaded throw an exception</exception>

    public void UpdateCallDetails(BO.Call CallUptade)
    {
        try
        {
            CallManager.ValidateCallDetails(CallUptade);

            // Update the latitude and longitude based on the validated address
            (CallUptade.CallLatitude, CallUptade.CallLongitude) = CallManager.GetCoordinatesFromAddress(CallUptade.CallAddress);
            // Convert the business object to a data object by calling a method
            DO.Call doCall=CallManager.ConvertToLogicCall(CallUptade);
            

            // Attempt to update the call in the data layer
            _dal.Call.Update(doCall);
        }
        catch (DO.DALException ex)
        {
            Console.WriteLine(ex.Message);
            throw new BO.BLException("An error occurred while updating the call details.", ex);
        }
    }

    /// <summary>
    /// Updates the end of treatment for a call assignment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    /// <exception cref="BO.BLException">Thrown when the request is invalid.</exception>
    public void EndTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);
            // Check if the volunteer is authorized to end the treatment
            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BLException("The volunteer is not authorized to end this treatment.");
            }

            // Check if the assignment is still open
            if (assignment.Begin != null)
            {
                throw new BO.BLException("The assignment has already been closed.");
            }

            // Update the assignment details using the 'with' expression
            var updatedAssignment = assignment with
            {
                End = DateTime.Now,
                MyEndStatus = DO.EndStatus.Completed,
            };

            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DALException ex)
        {
            // Handle data layer exceptions and rethrow as business layer exceptions
            throw new BO.BLException("An error occurred while ending the treatment.", ex);
        }
    }

    /// <summary>
    /// This method cancel's an assignment by receiving the Id of the requester and the id of the assignment to cancel
    /// </summary>
    /// <param name="requesterId">The resquester to cancel the id</param>
    /// <param name="assignmentId">The assignment that needs to be deleted</param>
    /// <exception cref="BO.BLDoesNotExistException">If the assignment with this Id does not exist throwing an exception</exception>
    /// <exception cref="BO.BLUnauthorizedException">If the requester is not a director or the volunteer that was assigned to this assignment throwing an exception</exception>
    /// <exception cref="BO.BLInvalidOperationException">If the Assignment has already been completed throw an exception </exception>
    /// <exception cref="BO.BLException">If for any reason the cancelation could not have been completed throw an exception</exception>
    public void CancelAssignment(int requesterId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
            {
                throw new BO.BLDoesNotExistException($"Assignment with Id {assignmentId} does not exist.");
            }

            // Check if the requester is authorized to cancel the assignment
            bool isAuthorized = requesterId == assignment.VolunteerId || CallManager.IsRequesterDirector(requesterId); 
            if (!isAuthorized)
            {
                throw new BO.BLUnauthorizedException("Requester is not authorized to cancel this assignment.");
            }

            // Check if the assignment is still open
            if (assignment.End != null)
            {
                throw new BO.BLInvalidOperationException("Cannot cancel an assignment that has already been completed.");
            }

            // Update the assignment details using the 'with' expression
            var updatedAssignment = assignment with
            {
                End = DateTime.Now,
                MyEndStatus = requesterId == assignment.VolunteerId ? DO.EndStatus.SelfCancelled : DO.EndStatus.DirectorCancelled
            };

            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DALException ex)
        {
            Console.WriteLine(ex.Message);
            throw new BO.BLException("An error occurred while canceling the assignment.", ex);
        }
    }

    /// <summary>
    /// This method assign's a call to a volunteer
    /// </summary>
    /// <param name="volunteerId">The volunteer that will take care of the call</param>
    /// <param name="callId">The id of the call</param>
    /// <exception cref="BO.BLDoesNotExistException">If the Id does not refer to any id throw an exception </exception>
    /// <exception cref="BO.BLInvalidOperationException">If the call has already been treated or has an open assignment throw an exception </exception>
    /// <exception cref="BO.BLException">>If for any reason the assignment could not have been completed throw an exceptio</exception>
    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        try
        {
            // Fetch the call from the data layer
            DO.Call? call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new BO.BLDoesNotExistException($"Call with Id {callId} does not exist.");
            }

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
/// creer une fonction pour verifier qye le end time quil donne est le meme parce que pas le droit de
/// לעדכן אותו
}
