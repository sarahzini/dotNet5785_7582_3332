using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.Json;
namespace Helpers;
/// <summary>
/// All the Helpers methods that are used for the implementations of calls.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5 
    /// <summary>
    /// This method converts a DO.Call object to a BO.CallInList object.
    /// </summary>
    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        IEnumerable<DO.Assignment>? assignments;
        lock (AdminManager.BlMutex)
            assignments = s_dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == call.CallId);
        DO.Assignment? assign = assignments?.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

        string? name;
        lock (AdminManager.BlMutex)
            name = s_dal.Volunteer.Read(volunteer => volunteer.VolunteerId == assign?.VolunteerId)?.Name;

        TimeSpan? exTime = null;
        if (assign?.MyEndStatus == DO.EndStatus.Completed) exTime = assign.End - call.OpenTime;

        BO.Statuses status = ToDeterminateStatus(assign, call);

        TimeSpan? rangeTimeToEnd=null;

        if (status== BO.Statuses.Expired) rangeTimeToEnd = TimeSpan.Zero;
        else if(status != BO.Statuses.Closed) rangeTimeToEnd= call.MaxEnd - s_dal.Config.Clock;

        return new BO.CallInList
        {
            AssignId = assign?.AssignmentId,
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            BeginTime = call.OpenTime,
            RangeTimeToEnd = rangeTimeToEnd,
            NameLastVolunteer = name,
            ExecutedTime = exTime,
            Status = status,
            TotalAssignment = assignments is null ? 0 : assignments.Count()
        };
    }
    /// <summary>
    /// This method converts a DO.Call object to a BO.ClosedCallInList.
    /// </summary>
    internal static BO.ClosedCallInList ConvertToClosedCallInList(DO.Call call,int id)
    {
        DO.Assignment? assign;
        lock (AdminManager.BlMutex)
            assign = s_dal.Assignment.Read(assignment => assignment.CallId == call.CallId&&assignment.VolunteerId==id&& assignment.End != null);
         

        return new BO.ClosedCallInList
        {
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            CallAddress = call.Address,
            BeginTime = call.OpenTime,
            BeginActionTime = assign!.Begin,
            EndActionTime = assign!.End,
            TypeOfEnd = (BO.EndStatus)assign.MyEndStatus!
        };
    }

    /// <summary>
    /// This method converts a DO.Call object to a BO.OpenCallInList.
    /// </summary>
    internal static BO.OpenCallInList ConvertToOpenCallInList(DO.Call call, int volunteerId)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex)
            volunteer = s_dal.Volunteer.Read(volunteerId);

        double distance = CalculOfDistance(call,volunteer);

        return new BO.OpenCallInList
        {
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            Description = call.Description,
            CallAddress = call.Address,
            BeginTime = call.OpenTime,
            MaxEndTime = call.MaxEnd,
            VolunteerDistanceToCall = distance
        };
    } 

    /// <summary>
    /// This method converts a BO.Call object to a DO.Call object.
    /// </summary>
    internal static DO.Call ConvertToDataCall(BO.Call call)
    {
        
        return new DO.Call
        {
            CallId = call.CallId,
            Address = call.CallAddress,
            Latitude = call.CallLatitude,
            Longitude = call.CallLongitude,
            OpenTime = call.BeginTime,
            AmbulanceType = (DO.SystemType)call.TypeOfCall,
            Description = call.Description,
            MaxEnd = call.MaxEndTime
        };
    }

    /// <summary>
    /// This method converts a DO.Call object to a BO.Call object.
    /// </summary>
    internal static BO.Call ConvertToLogicCall(DO.Call call)
    {

        // Get the call assignments from the data layer
        IEnumerable<DO.Assignment>? assignments;
        lock (AdminManager.BlMutex)
            assignments = s_dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == call.CallId);

        DO.Assignment? assign = assignments?.OrderByDescending(assignment => assignment.Begin).FirstOrDefault();
        BO.Statuses status = ToDeterminateStatus(assign, call);

        List<BO.CallAssignInList>? CallAssignInList = assignments?
            .Select(assignment => new BO.CallAssignInList
            {
                VolunteerId = assignment.VolunteerId,
                VolunteerName = s_dal.Volunteer.Read(assignment.VolunteerId)?.Name,
                BeginActionTime = assignment.Begin,
                EndActionTime = assignment.End,
                ClosureType = assignment.End is null ? null:(BO.EndStatus)assignment!.MyEndStatus!
            })
            .ToList();

        return new BO.Call
        {
            CallId = call.CallId,
            TypeOfCall = (BO.SystemType)call.AmbulanceType,
            Description = call.Description,
            CallAddress = call.Address,
            CallLatitude = call.Latitude,
            CallLongitude = call.Longitude,
            BeginTime = call.OpenTime,
            MaxEndTime = call.MaxEnd,
            Status = status,
            CallAssigns = CallAssignInList
        };
    }

    /// <summary>
    ///This method determine the status of a call, depending on the case it can be Open, Closed, InAction, InActionToRisk, OpenToRisk, Expired.
    /// </summary>
    private static BO.Statuses ToDeterminateStatus(DO.Assignment? assign, DO.Call call)
    {
        if (assign == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd) { return BO.Statuses.Expired; }
        else if (assign == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange)
        { return BO.Statuses.OpenToRisk; }
        else if (assign == null) { return BO.Statuses.Open; }
        else if (assign?.End == null && call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange)
        { return BO.Statuses.InActionToRisk; }
        else if (assign?.End == null) { return BO.Statuses.InAction; }
        else if (assign.MyEndStatus == DO.EndStatus.Completed) { return BO.Statuses.Closed; }
        else if (assign.MyEndStatus == DO.EndStatus.Expired) { return BO.Statuses.Expired; }
        //else this is Manager or SelfCancelled so we need to check the time 
        else if(call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd) { return BO.Statuses.Expired; }
        else if (call.MaxEnd.HasValue && s_dal.Config.Clock > call.MaxEnd - s_dal.Config.RiskRange) { return BO.Statuses.OpenToRisk; }
        else { return BO.Statuses.Open; }

    }

    /// <summary>
    /// This method determine if the requester is a manager.
    /// </summary>
    internal static bool IsRequesterManager(int requesterId)
    {
        DO.Volunteer? user;
        lock (AdminManager.BlMutex)
                user= s_dal.Volunteer.Read(requesterId);
        return user is null ? false : user?.MyJob == DO.Job.Manager;
    }

    /// <summary>
    /// This method checks the details of a call.
    /// </summary>
    internal static void ValidateCallDetails(BO.Call call)
    {
        //checks wether the Maxendtime is greater than the call start time and the current time
        if (call.MaxEndTime <= call.BeginTime || call.MaxEndTime <= AdminManager.Now)
        {
            throw new BO.BLFormatException("The maximum end time must be greater than the call start time and the current time.");
        }
        if (call.TypeOfCall != BO.SystemType.ICUAmbulance && call.TypeOfCall != BO.SystemType.RegularAmbulance)
        {
            throw new BO.BLFormatException("The call must be ICU or Regular type.");
        }

        //the adress details will be check after the call of this function
    }

    internal static double CalculOfDistance(DO.Call call, DO.Volunteer? volunteer )
    {
        
        double? lat1 = volunteer!.Latitude!;
        double? lon1 = volunteer!.Longitude!;
        double? lat2 = call.Latitude!;
        double? lon2 = call.Longitude!;

        double dLat = (Math.PI / 180)*(double)(lat2 - lat1);
        double dLon = (Math.PI / 180) * (double)(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos((Math.PI / 180) * (double)(lat1)) * Math.Cos((Math.PI / 180) * (double)(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return Math.Round(6371 * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)), 2); // 6371 is the radius of the earth in km, rounded to 2 decimal places

    }
}

