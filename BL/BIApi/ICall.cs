
using Helpers;

namespace BIApi;
 public interface  ICall
{
    /// <summary>
    /// Alls the Methods used, they are implemented in the CallImplementation class
    /// </summary>
    public int[] TypeOfCallCounts();
    public IEnumerable<BO.CallInList> SortCalls(BO.CallInListField? filterField, object? filterValue, BO.CallInListField? sortField);
    public BO.Call GetCallDetails(int CallId);
    public void UpdateCallDetails(BO.Call CallUptade);
    public void EndTreatment(int volunteerId, int assignmentId);
    public void CancelAssignment(int requesterId, int assignmentId);
    public void AssignCallToVolunteer(int volunteerId, int callId);
    public void DeleteCall(int callId);
    public void AddCall(BO.Call call);
    public IEnumerable<BO.OpenCallInList> SortOpenCalls(int volunteerId, BO.SystemType? callType, BO.CallInListField? sortField);
    public IEnumerable<BO.ClosedCallInList> SortClosedCalls(int volunteerId, BO.SystemType? callType, BO.ClosedCallInListField? sortField);

}
