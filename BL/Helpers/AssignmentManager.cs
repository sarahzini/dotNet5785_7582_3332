using DalApi;

namespace Helpers;
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get;
    internal static DO.Assignment SearchAssignment(Func<DO.Assignment, bool> predicate)
    {
        DO.Assignment? assignment = s_dal.Assignment.Read(predicate);
        if (assignment == null)
        {
            throw new BO.BLDoesNotExistException("Assignment matching with this criteria does not exist");
        }
        return assignment;

    }
}
