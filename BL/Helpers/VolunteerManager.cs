using BO;
using DalApi;
using DO;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static DO.Volunteer SearchVolunteer(Func<DO.Volunteer, bool> predicate)
    {
        DO.Volunteer? volunteer = s_dal.Volunteer.Read(predicate);
        if (volunteer == null)
        {
            throw new DO.DalDoesNotExistException("Volunteer matching the specified criteria does not exist");
        }
        return volunteer;

    }

    internal static IEnumerable<DO.Volunteer> GetVolunteers(Func<DO.Volunteer, bool>? predicate)
    {
        IEnumerable<DO.Volunteer>? volunteers = s_dal.Volunteer.ReadAll(predicate);
        if (volunteers == null)
        {
            throw new DO.DalDoesNotExistException("Volunteers matching the specified criteria don't exist");
        }
        return volunteers;

    }
}
