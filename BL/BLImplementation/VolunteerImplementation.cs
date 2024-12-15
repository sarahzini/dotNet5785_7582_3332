
using BIApi;
using Helpers;

namespace BLImplementation;

internal class VolunteerImplementation:IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    DO.Job Login(string name, string password)
    {
        try
        {
            DO.Volunteer volunteer = VolunteerManager.SearchVolunteer(volunteer => volunteer.Name == name);
            if (volunteer.password != password)
            {
                throw new BO.BLIncorrectPassword($"Incorrect password");
            }
            return volunteer.MyJob;
        }
        catch (BO.BLDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException($"Volunteer with this name does not exist");
        }
        catch (BO.BLIncorrectPassword ex)
        {
            throw new BO.BLIncorrectPassword($"Incorrect password");
        }
    }

    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.VolunteerFieldSort? sortField)
    {
        try
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
        catch (BO.BLDoesNotExistException ex)
        {
            throw new BO.BLDoesNotExistException("Volunteers matching the specified criteria don't exist");
        }
    }

    BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            DO.Volunteer volunteer = VolunteerManager.SearchVolunteer(volunteer => volunteer.Id == volunteerId);
            DO.CallInProgress? callInProgress = VolunteerManager.GetCallInProgress(volunteerId);

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
                VolunteerJob = volunteer.MyJob,
                IsActive = volunteer.active,
                MaxVolunteerDistance = volunteer.distance,
                TransportType = volunteer.MyWhichDistance,
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
