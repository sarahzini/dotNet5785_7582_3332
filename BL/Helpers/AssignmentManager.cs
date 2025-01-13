using DalApi;

namespace Helpers;
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5

    /// <summary>
    /// This method updates the assignments based on the new clock.
    /// </summary>
    internal static void PeriodicAssignmentsUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll();

        if (assignments == null)
            return;

        bool assignmentUpdated = false; //stage 5

        foreach (var assignment in assignments)
        {
            if (assignment.End == null)
            {
                if (s_dal.Call.Read(c => c.CallId == assignment.CallId)?.MaxEnd < newClock)
                {
                    assignmentUpdated = true;
                    DO.Assignment newAssign = assignment with { MyEndStatus = DO.EndStatus.Expired };
                    s_dal.Assignment.Update(newAssign);
                    Observers.NotifyItemUpdated(newAssign.AssignmentId); //stage 5
                }
            }
        }
        if ( assignmentUpdated) //stage 5
            Observers.NotifyListUpdated(); //stage 5
    }

}

