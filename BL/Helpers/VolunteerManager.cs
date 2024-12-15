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

    internal static BO.VolunteerInList ConvertToVolunteerInList(DO.Volunteer v)
    {
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);

        int completedCount=0, canceledCount=0, expiredCount=0;

        foreach (var a in assignments)
        {
            _ = a.MyEndStatus switch
            {
                DO.EndStatus.Completed => completedCount++,
                DO.EndStatus.SelfCancelled => canceledCount++,
                DO.EndStatus.ManagerCancelled => canceledCount++,
                DO.EndStatus.Expired => expiredCount++
            };
        }


        return new BO.VolunteerInList
        {
            VolunteerId=v.Id,
            Name=v.Name, 
            IsActive=v.active,
            CompletedCalls = completedCount,
            CanceledCalls = canceledCount,
            ExpiredCalls = expiredCount,
            ActualCallId = assignments.FirstOrDefault(a => a.End == null)?.CallId,
            TypeOfCall = (BO.SystemType)s_dal.Call.Read(c => c.Id == assignments.First().CallId).Choice 
        };
    }
}
