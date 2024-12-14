using BO;

namespace BIApi;
public partial interface ICall
{
    IEnumerable<BO.CallInList> GetFilteredAndSortedCalls(CallInListField? filterField, object? filterValue, CallInListField? sortField);
    void DeleteCall(int callId);
    public void AddCall(BO.Call call);
    public IEnumerable<BO.CallInList> GetOpenCallsForVolunteer(int volunteerId, SystemType? callType, CallInListField? sortField);
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.SystemType? callType, ClosedCallInListField? sortField);
}