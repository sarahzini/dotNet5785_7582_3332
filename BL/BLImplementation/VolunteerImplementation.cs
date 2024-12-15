
using BIApi;
using DalApi;
using Helpers;

namespace BLImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    DO.Job Login(string name, string password)
    {
        try
        {
            DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteer => volunteer.Name == name);

            if (volunteer?.password != password)
            {
                throw new BO.BLIncorrectPassword($"Incorrect password");
            }
            return volunteer.MyJob;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Volonteer {name} does not exist in the system");
        }
    }
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.VolunteerInListFieldSort? sortField)
    {
        try
        {
            // Get the full list of volunteers
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();

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
            return volunteers.Select(v => VolunteerManager.ConvertToVolunteerInList(v));  

        }
        catch (BO.BLDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("Volunteers matching the specified criteria don't exist");
        }
    }
    BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteerId);

            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

            int completedCount = 0, canceledCount = 0, expiredCount = 0, actualCallId = 0;

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

            DO.Assignment assign = _dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.End == null);
            DO.Call callInProgress = _dal.Call.Read(assign.CallId);

            double distance = Math.Sqrt(
       Math.Pow((double)(volunteer.Longitude - callInProgress.Longitude), 2) +
       Math.Pow((double)(volunteer.Latitude - callInProgress.Latitude), 2));


            return new BO.Volunteer
            {
                VolunteerId = volunteer.Id,
                Name = volunteer.Name,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Password = volunteer.password,
                VolunteerAddress = volunteer.Adress,
                VolunteerLatitude = volunteer.Latitude,
                VolunteerLongitude = volunteer.Longitude,
                VolunteerJob = (BO.Job)volunteer.MyJob,
                IsActive = volunteer.active,
                MaxVolunteerDistance = volunteer.distance,
                TransportType = (BO.WhichDistance)volunteer.MyWhichDistance,
                CompletedCalls =completedCount,
                CancelledCalls = canceledCount,
                ExpiredCalls = expiredCount,
                CurrentCall = callInProgress != null ? new BO.CallInProgress
                {
                    AssignId = assign.Id,
                    CallId = assign.CallId,
                    TypeOfCall = (BO.SystemType)callInProgress.Choice,
                    Description = callInProgress.Description,
                    CallAddress = callInProgress.Address,
                    BeginTime = callInProgress.DateTime,
                    MaxEndTime = callInProgress.EndDateTime,
                    BeginActionTime = assign.Begin,
                    VolunteerDistanceToCall = distance,
                    ClosureType = callInProgress.EndDateTime - DateTime.Now <= _dal.Config.RiskRange ? BO.Statuses.InActionToRisk : BO.Statuses.InAction
                } : null
            };

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
        }
    } 
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            DO.Volunteer requesterVolunteer = VolunteerManager.SearchVolunteer(volunteer => volunteer.Id == requesterId);
            DO.Job requesterJob = requesterVolunteer.MyJob;

            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            // Convert BO.Volunteer to DO.Volunteer
            DO.Volunteer updatedVolunteer = VolunteerManager.ConvertToDataVolunteer(volunteer);
            DO.Volunteer oldVolunteer = VolunteerManager.SearchVolunteer(oldVolunteer => oldVolunteer.Id == volunteer.VolunteerId);

            // Check if the requester is authorized to update member in volunteer 
            if (updatedVolunteer.MyJob!= updatedVolunteer.MyJob && requesterJob==DO.Job.Volunteer)
            {
                throw new BLInvalidOperationException("Requester is not authorized to update the volunteer");
            }

            // Attempt to update the volunteer in the data layer
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (BO.BLDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Volunteer with the Id {requesterId} does not exist");
        }
        catch (BO.BLInvalidOperationException ex)
        {
            throw new BO.BLInvalidOperationException(ex.Message);
        }
        catch (BO.BLFormatException ex)
        {
            throw new BO.BLFormatException(ex.Message);
        }

    }   
    public void DeleteVolunteer(string volunteerId)
    {
        try
        {
            // Check if the volunteer exists
            DO.Volunteer volunteer = VolunteerManager.SearchVolunteer(volunteer => volunteer.Id == int.Parse(volunteerId));

            //verifier si il peu etre supp


            // Attempt to delete the volunteer from the data layer
            _dal.Volunteer.Delete(int.Parse(volunteerId));
        }
        catch (BO.BLDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Volunteer with the Id {volunteerId} does not exist");
        }
    }  
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            // Convert BO.Volunteer to DO.Volunteer
            DO.Volunteer newVolunteer = VolunteerManager.ConvertToDataVolunteer(volunteer);

            // Attempt to add the new volunteer to the data layer
            _dal.Volunteer.Create(newVolunteer);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BLAlreadyExistException(ex.Message);
        }
        catch (BO.BLFormatException ex)
        {
            throw new BO.BLFormatException(ex.Message);
        }
    }
}
