using DalApi;

namespace Helpers;
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// This method updates the assignments based on the new clock.
    /// </summary>
    internal static void PeriodicAssignmentsUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll();

        if (assignments == null)
            return;

        foreach (var assignment in assignments)
        {
            if (s_dal.Call.Read(c=>c.CallId==assignment.CallId)?.MaxEnd < newClock)
            {
               DO.Assignment newAssign = assignment with { MyEndStatus = DO.EndStatus.Expired };
               s_dal.Assignment.Update(newAssign);
            }
        }
    }
}
