using BO;
using DO;
using Helpers;

namespace BIApi;

public interface IVolunteer
{
    DO.Job EnterSystem(string name, string password)
    {
        DO.Volunteer volunteer = VolunteerManager.SearchVolunteer(volunteer => volunteer.Name == name);
        if (volunteer.password != password)
        {
            throw new DO.DalDoesNotExistException($"Volunteer with name={name} and provided password does not exist");
        }
        return volunteer.MyJob;
    }

    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, VolunteerFieldSort? sortField = null)
    {
        // Get the full list of volunteers
        IEnumerable<DO.Volunteer> volunteers = VolunteerManager.GetVolunteers(null);

        // Filter the list based on the isActive parameter
        if (isActive.HasValue)
        {
            volunteers = volunteers.Where(v => v.active == isActive.Value);
        }

        // Sort the list based on the sortField parameter
        if (sortField.HasValue)
        {
            volunteers = volunteers.OrderBy(v => v.GetType().GetProperty(sortField.ToString()).GetValue(v, null));
        }
        else
        {
            volunteers = volunteers.OrderBy(v => v.Id);
        }

        // Convert the list to BO.VolunteerInList and return
        return volunteers.Select(v => new BO.VolunteerInList
        {
            VolunteerId = v.Id,
            Name = v.Name,
            IsActive = v.active
        });
    }

    BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        DO.Volunteer volunteer = VolunteerManager.SearchVolunteer(volunteerId);
        DO.CallInProgress? callInProgress = VolunteerManager.GetCallInProgress(volunteerId);

        return new BO.Volunteer
        {
            VolunteerId = volunteer.VolunteerId,
            Name = volunteer.Name,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            Password = volunteer.Password,
            VolunteerAddress = volunteer.VolunteerAddress,
            VolunteerLatitude = volunteer.VolunteerLatitude,
            VolunteerLongitude = volunteer.VolunteerLongitude,
            VolunteerJob = volunteer.VolunteerJob,
            IsActive = volunteer.IsActive,
            MaxVolunteerDistance = volunteer.MaxVolunteerDistance,
            TransportType = volunteer.TransportType,
            CompletedCalls = volunteer.CompletedCalls,
            CancelledCalls = volunteer.CancelledCalls,
            ExpiredCalls = volunteer.ExpiredCalls,
            CurrentCall = callInProgress != null ? new BO.CallInProgress
            {
                AssignId = callInProgress.AssignId,
                CallId = callInProgress.CallId,
                TypeOfCall = callInProgress.TypeOfCall,
                Description = callInProgress.Description,
                CallAddress = callInProgress.CallAddress,
                BeginTime = callInProgress.BeginTime,
                MaxEndTime = callInProgress.MaxEndTime,
                BeginActionTime = callInProgress.BeginActionTime,
                VolunteerDistanceToCall = callInProgress.VolunteerDistanceToCall,
                ClosureType = callInProgress.ClosureType
            } : null
        };
    }
}
