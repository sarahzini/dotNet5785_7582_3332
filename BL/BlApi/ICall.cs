using BO;
using DO;
using Helpers;

namespace BlApi;
 public interface ICall: IObservable
{
    /// <summary>
    /// Alls the Methods that are used, they are implemented in the CallImplementation class
    /// </summary>
    public int[] TypeOfCallCounts();
    public IEnumerable<BO.CallInList>? GetSortedCallsInList(BO.CallInListField? filterField=null, object? filterValue=null, BO.CallInListField? sortField=null);
    public BO.Call GetCallDetails(int CallId); 
    public void UpdateCallDetails(BO.Call CallUptade);
    public void DeleteCall(int callId);
    public void AddCall(BO.Call call);
    public IEnumerable<BO.ClosedCallInList>? SortClosedCalls(int volunteerId, BO.SystemType? callType, BO.ClosedCallInListField? sortField);
    public IEnumerable<BO.OpenCallInList>? SortOpenCalls(int volunteerId, BO.SystemType? callType, BO.OpenCallInListField? sortField);
    public void CompleteCall(int volunteerId, int assignmentId); 
    public void CancelAssignment(int requesterId, int? assignmentId); 
    public void AssignCallToVolunteer(int volunteerId, int callId); 

}
