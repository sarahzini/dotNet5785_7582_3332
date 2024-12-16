
using BIApi;
using BO;
using DO;
using Helpers;
using System.Xml.Linq;

namespace BLImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;  
    DO.Job IVolunteer.Login(string name, string password)
    {
        try
        {
            DO.Volunteer volunteer = _dal.Volunteer.Read(volunteer => volunteer.Name == name);

            if (volunteer.Password != password)
            {
                throw new BO.BLIncorrectPassword($"Incorrect Password");
            }
            return volunteer.MyJob;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured with {name}", ex);
        }
    }
    IEnumerable<VolunteerInList> IVolunteer.GetVolunteersInList(bool? isActive, VolunteerInListFieldSort? sortField)
    {
        try
        {
            // Get the full list of volunteers
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();

            // Filter the list based on the isActive parameter
            if (isActive.HasValue)
            {
                volunteers = volunteers.Where(v => v.IsActive == isActive.Value);
            }

            // Sort the list based on the sortField parameter
            if (sortField.HasValue)
            {
                volunteers = volunteers.OrderBy(v => v.GetType().GetProperty(sortField.ToString()).GetValue(v, null));
            }
            else
            {
                volunteers = volunteers.OrderBy(v => v.VolunteerId);
            }

            // Convert the list to BO.VolunteerInList and return
            return volunteers.Select(v => VolunteerManager.ConvertToVolunteerInList(v));

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("An error occured :",ex);
        }
    }
    BO.Volunteer IVolunteer.GetVolunteerDetails(int volunteerId)
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
                    Status = callInProgress.OpenTime - _dal.Config.Clock <= _dal.Config.RiskRange ? BO.Statuses.InActionToRisk : BO.Statuses.InAction
                } : null
            };

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured: ",ex);
        }

    } //reorganiser pr que la fct soit petite ET calculer la distance
    public void UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            if (requesterId != volunteer.VolunteerId || !CallManager.IsRequesterManager(requesterId))
            {
                throw new BLInvalidOperationException("Requester is not authorized to update the volunteer");
            }

            // Validate volunteer details
            VolunteerManager.ValidateVolunteerDetails(volunteer);

            DO.Volunteer oldVolunteer = _dal.Volunteer.Read(volunteer.VolunteerId);
            DO.Volunteer updatedVolunteer = VolunteerManager.ConvertToDataVolunteer(volunteer);

            VolunteerManager.CheckAuthorisationToUpdate(oldVolunteer, updatedVolunteer, CallManager.IsRequesterManager(requesterId));

            // Attempt to update the volunteer in the data layer
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured with {volunteer.Name}: ", ex);
        }
    }
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            IEnumerable<DO.Assignment>? assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

            if (assignments?.Any(a=>a.End == null)==true)
            {
                throw new BLInvalidOperationException($"Volunteer {volunteerId} is currently assigned to a call and cannot be deleted");
            }

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"An error occured : we cannot delete the volunteer with the ID {volunteerId}. ", ex);
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
            throw new BO.BLAlreadyExistException($"An error occured : we cannot add the volunteer with the ID {volunteer.VolunteerId}.",ex);
        }
    }
}
