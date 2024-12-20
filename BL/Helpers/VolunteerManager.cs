using BO;
using DalApi;
using DO;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// This method validates the details of a volunteer by checking the email, ID, name, phone number, password, job, and address
    /// with the requirements of each members.
    /// </summary>
     internal static void ValidateVolunteerDetails(BO.Volunteer volunteer)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(volunteer.Email))
        {
            throw new BO.BLFormatException("Invalid email format.");
        }

        if (!int.TryParse(volunteer.VolunteerId.ToString(), out int parsedId) || volunteer.VolunteerId.ToString().Length != 9)
        {
            throw new BO.BLFormatException("ID must contain exactly 9 digits.");
        }

        var nameRegex = new Regex(@"^[a-zA-Z]+$");
        if (!nameRegex.IsMatch(volunteer.Name))
        {
            throw new BO.BLFormatException("Name must contain only letters.");
        }

        var phoneRegex = new Regex(@"^\d{10}$");
        if (!phoneRegex.IsMatch(volunteer.PhoneNumber))
        {
            throw new BO.BLFormatException("Phone number must contain exactly 10 digits.");
        }
        var PasswordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d).+$");
        if (!string.IsNullOrEmpty(volunteer.Password) && !PasswordRegex.IsMatch(volunteer.Password))
        {
            throw new BO.BLFormatException("Password must contain at least one digit and one uppercase letter.");
        }

        if (volunteer.VolunteerJob != BO.Job.Manager && volunteer.VolunteerJob != BO.Job.Volunteer)
        {
            throw new BO.BLFormatException("Job must be either 'Manager' or 'Volunteer'.");
        }
    }

    /// <summary>
    /// This method converts a BO.Volunteer object to a DO.Volunteer object.
    /// </summary>
    internal static DO.Volunteer ConvertToDataVolunteer(BO.Volunteer volunteer)
    {
        (double latitude, double longitude) = CallManager.GetCoordinatesFromAddress(volunteer.VolunteerAddress);
        return new DO.Volunteer
        {
            VolunteerId = volunteer.VolunteerId,
            Name = volunteer.Name,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.VolunteerAddress,
            Latitude = latitude,
            Longitude = longitude,
            MyJob = (DO.Job)volunteer.VolunteerJob,
            IsActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxVolunteerDistance,
            MyDistanceType = (DO.DistanceType)volunteer.VolunteerDT
        };
    }

    /// <summary>
    /// This method converts a DO.Volunteer List to a BO.Volunteer List.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    internal static BO.VolunteerInList ConvertToVolunteerInList(DO.Volunteer v)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.VolunteerId);

        int completedCount=0, canceledCount=0, expiredCount=0;
        int nothing = 0;

        if (assignments != null)
        {
            foreach (var a in assignments)
            {
                _ = a.MyEndStatus switch
                {
                    DO.EndStatus.Completed => completedCount++,
                    DO.EndStatus.SelfCancelled => canceledCount++,
                    DO.EndStatus.ManagerCancelled => canceledCount++,
                    DO.EndStatus.Expired => expiredCount++,
                    _ => nothing++
                };
            }
        }

        int? actualCallId = assignments?.FirstOrDefault(a => a.End == null)?.CallId;
        BO.SystemType typeOfCall = actualCallId is null ? BO.SystemType.None :
            (s_dal.Call.Read(c => c.CallId == actualCallId) is null ? 
            BO.SystemType.None : (BO.SystemType)s_dal.Call.Read(c => c.CallId == actualCallId).AmbulanceType);

        return new BO.VolunteerInList
        {
            VolunteerId=v.VolunteerId,
            Name=v.Name, 
            IsActive=v.IsActive,
            CompletedCalls = completedCount,
            CanceledCalls = canceledCount,
            ExpiredCalls = expiredCount,
            ActualCallId = actualCallId,
            TypeOfCall = typeOfCall
        };
    }

    /// <summary>
    /// This method converts a DO.Volunteer object to a BO.Volunteer object.
    /// </summary>
    internal static BO.Volunteer ConvertToLogicalVolunteer(DO.Volunteer volunteer, int volunteerId)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

        int completedCount = 0, canceledCount = 0, expiredCount = 0, nothing = 0;

        if (assignments != null)
        {
            foreach (var a in assignments)
            {
                _ = a.MyEndStatus switch
                {
                    DO.EndStatus.Completed => completedCount++,
                    DO.EndStatus.SelfCancelled => canceledCount++,
                    DO.EndStatus.ManagerCancelled => canceledCount++,
                    DO.EndStatus.Expired => expiredCount++,
                    _ => nothing++
                };
            }
        }

        DO.Assignment? assign = s_dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.End == null);
        DO.Call? callInProgress = assign is null ? null : s_dal.Call.Read(assign.CallId);

        (double latitude, double longitude) = CallManager.GetCoordinatesFromAddress(volunteer.Address);



        double distance = Math.Sqrt(
   Math.Pow((double)(longitude - callInProgress.Longitude), 2) +
   Math.Pow((double)(latitude - callInProgress.Latitude), 2));


        return new BO.Volunteer
        {
            VolunteerId = volunteer.VolunteerId,
            Name = volunteer.Name,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            Password = volunteer.Password,
            VolunteerAddress = volunteer.Address,
            VolunteerLatitude = volunteer.Latitude,
            VolunteerLongitude = volunteer.Longitude,
            VolunteerJob = (BO.Job)volunteer.MyJob,
            IsActive = volunteer.IsActive,
            MaxVolunteerDistance = volunteer.MaxDistance,
            VolunteerDT = (BO.DistanceType)volunteer.MyDistanceType,
            CompletedCalls = completedCount,
            CancelledCalls = canceledCount,
            ExpiredCalls = expiredCount,
            CurrentCall = callInProgress != null ? new BO.CallInProgress
            {
                AssignId = assign.AssignmentId,
                CallId = assign.CallId,
                TypeOfCall = (BO.SystemType)callInProgress.AmbulanceType,
                Description = callInProgress.Description,
                CallAddress = callInProgress.Address,
                BeginTime = callInProgress.OpenTime,
                MaxEndTime = callInProgress.MaxEnd,
                BeginActionTime = assign.Begin,
                VolunteerDistanceToCall = distance,
                Status = callInProgress.OpenTime - s_dal.Config.Clock <= s_dal.Config.RiskRange ? BO.Statuses.InActionToRisk : BO.Statuses.InAction
            } : null
        };
    }

    /// <summary>
    /// This method checks if the volunteer is authorized to update their details.
    /// </summary>
    internal static void CheckAuthorisationToUpdate(DO.Volunteer? oldVolunteer, DO.Volunteer updatedVolunteer, bool isManager)
    {
        if (oldVolunteer?.Name != updatedVolunteer.Name)
        { throw new BLInvalidOperationException("Name cannot be changed."); }
        if(oldVolunteer?.MyJob != updatedVolunteer.MyJob&& !isManager)
        { throw new BLInvalidOperationException("Job cannot be changed by volunteer."); }
        //bonus : we authorized the volunteer to change his transport type
    }

}
