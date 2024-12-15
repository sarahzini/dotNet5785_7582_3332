using DalApi;

namespace Helpers;
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get;
    internal static DO.Assignment? SearchAssignment(Func<DO.Assignment, bool> predicate)
    {
        DO.Assignment? assignment = s_dal.Assignment.Read(predicate);
        return assignment;
    }
}
