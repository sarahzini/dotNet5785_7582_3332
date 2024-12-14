using BO;
using DalApi;
using DO;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static DO.Volunteer SearchVolunteer(Func<DO.Volunteer, bool> predicate)
    {
            DO.Volunteer? volunteer = s_dal.Volunteer.Read(predicate);
            if (volunteer == null)
            {
                throw new BO.BLDoesNotExistException("Volunteer matching with this criteria does not exist");
            }
            return volunteer;

    }
    internal static IEnumerable<DO.Volunteer> GetVolunteers(Func<DO.Volunteer, bool>? predicate)
    {
        IEnumerable<DO.Volunteer>? volunteers = s_dal.Volunteer.ReadAll(predicate);
        if (volunteers == null)
        {
            throw new BO.BLDoesNotExistException("Volunteers matching the specified criteria don't exist");
        }
        return volunteers;

    }

    internal static void ValidateVolunteerDetails(BO.Volunteer volunteer)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(volunteer.Email))
        {
            throw new BO.BLFormatException("Invalid email format.");
        }

        if (!int.TryParse(volunteer.VolunteerId.ToString(), out int parsedId) || volunteer.VolunteerId.ToString().Length != 9)
        {
            throw new BO.BLFormatException("Invalid ID format.");
        }
    }

    // Helper method to convert BO.Volunteer to DO.Volunteer
    internal static DO.Volunteer ConvertToDataVolunteer(BO.Volunteer volunteer)
    {
        return new DO.Volunteer
        {
            Id = volunteer.VolunteerId,
            Name = volunteer.Name,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            password = volunteer.Password,
            Adress = volunteer.VolunteerAddress,
            Latitude = volunteer.VolunteerLatitude,
            Longitude = volunteer.VolunteerLongitude,
            MyJob = (DO.Job)volunteer.VolunteerJob,
            active = volunteer.IsActive,
            distance = volunteer.MaxVolunteerDistance,
            MyWhichDistance = (DO.WhichDistance)volunteer.TransportType
        };
    }
}
