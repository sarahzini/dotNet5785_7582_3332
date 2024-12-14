namespace BIApi;
 public partial interface  ICall
{
    /// <summary>
    /// Alls the Methods used, they are implemented in the CallImplementation class
    /// </summary>
    public int[] GetCallCounts();
    public BO.Call GetCallDetails(int CallId);
    public void UpdateCallDetails(BO.Call CallUptade);
    public void EndTreatment(int volunteerId, int assignmentId);
    public void CancelAssignment(int requesterId, int assignmentId);
    public void AssignCallToVolunteer(int volunteerId, int callId);
 }
