using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using static Helpers.CallManager;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    /// <summary>
    /// This method validates the details of a volunteer by checking the email, ID, name, phone number, password, job, and address
    /// with the requirements of each members.
    /// </summary>
    internal static void ValidateVolunteerDetails(BO.Volunteer volunteer)
    {
        var nameRegex = new Regex(@"^[a-zA-Z\s]+$");
        if (!nameRegex.IsMatch(volunteer.Name))
        {
            throw new BO.BLFormatException("Name must contain only letters and spaces.");
        }
        if (!int.TryParse(volunteer.VolunteerId.ToString(), out int parsedId) || (volunteer.VolunteerId.ToString().Length != 9 && volunteer.VolunteerId.ToString().Length != 8))
        {
            throw new BO.BLFormatException("ID must contain exactly 8-9 digits.");
        }
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(volunteer.Email))
        {
            throw new BO.BLFormatException("Invalid email format.");
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
        return new DO.Volunteer
        {
            VolunteerId = volunteer.VolunteerId,
            Name = volunteer.Name,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.VolunteerAddress,
            Latitude = volunteer.VolunteerLatitude,
            Longitude = volunteer.VolunteerLongitude,
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
        IEnumerable<DO.Assignment>? assignments;

        lock (AdminManager.BlMutex)
             assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.VolunteerId);

        int completedCount=0, canceledCount=0, expiredCount=0;
        int nothing = 0;

        if (assignments != null)
        {
            foreach (var a in assignments)
            {
                if (a.MyEndStatus != null)
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
        }

        int? actualCallId = assignments?.FirstOrDefault(a => a.End == null)?.CallId;

        BO.SystemType typeOfCall;

        lock (AdminManager.BlMutex)
            typeOfCall = actualCallId is null ? BO.SystemType.None :
            (BO.SystemType)((s_dal.Call.Read(c => c.CallId == actualCallId)!.AmbulanceType));

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
        IEnumerable<DO.Assignment>? assignments;

        lock (AdminManager.BlMutex)
            assignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

        int completedCount = 0, canceledCount = 0, expiredCount = 0, nothing = 0;

        if (assignments != null)
        {
            foreach (var a in assignments)
            {
                _ = a.MyEndStatus switch
                {
                    DO.EndStatus.Completed => completedCount++,
                    DO.EndStatus.SelfCancelled => canceledCount++,
                    DO.EndStatus.Expired => expiredCount++,
                    _ => nothing++
                };
            }
        }

        DO.Assignment? assign;
        DO.Call? callInProgress;

        lock (AdminManager.BlMutex)
        {
            assign = s_dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.End == null);
            callInProgress = assign is null ? null : s_dal.Call.Read(assign.CallId);
        }

        double distance = 0;
        if (callInProgress != null)
        {
             distance = CallManager.CalculOfDistance(callInProgress,volunteer);
        }


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
                AssignId = assign!.AssignmentId,
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
        //if (oldVolunteer?.Name != updatedVolunteer.Name)
        //{ throw new BLInvalidOperationException("Name cannot be changed."); }
        if(oldVolunteer?.MyJob != updatedVolunteer.MyJob&& !isManager)
        { throw new BLInvalidOperationException("You cannot change this member because you are not a Manager."); }
    }

    internal static void SimulateVolunteerSystem()
    {
        //verifier qu il l appelle bien la fct qui voient si ils sont expires avant de rentrer ici 
        bool volunteersUpdate = false;
        Random random = new Random();

        IEnumerable<DO.Volunteer>? volunteers;
        List<DO.Call>? calls;
        IEnumerable<DO.Assignment>? assignments;
        lock (AdminManager.BlMutex)
        {
            volunteers = s_dal.Volunteer.ReadAll()!.Where(v => v.IsActive == true).ToList();
            calls = s_dal.Call.ReadAll()!.ToList();
            assignments = s_dal.Assignment.ReadAll()!.ToList();
        }

        DateTime now;
        lock (AdminManager.BlMutex)
            now = AdminManager.Now;

        List<DO.Call> filteredCalls = new List<DO.Call>(calls!);

        foreach (var C in calls!)
        {
            DO.Assignment? a;
            lock (AdminManager.BlMutex)
                a = s_dal.Assignment.ReadAll()?.Where(assignment => assignment.CallId == C.CallId).OrderByDescending(assignment => assignment.Begin).FirstOrDefault();

            if (a != null && a.End != null)
            {
                if (a.MyEndStatus == DO.EndStatus.Completed || a.MyEndStatus == DO.EndStatus.Expired)
                {
                    filteredCalls.Remove(C);
                    continue;

                }
            }
            if (C.MaxEnd != null && C.MaxEnd < AdminManager.Now)
            {
                filteredCalls.Remove(C);
                continue;

            }
            if (a != null && a?.End == null)
            {
                filteredCalls.Remove(C);
                continue;

            }
        }

        // Now filteredCalls contains only the calls that meet the criteria
        calls = filteredCalls;

        int i = 0;
        foreach (var v in volunteers)
        {
            var currentAssignment = assignments.FirstOrDefault(a => a.VolunteerId == v.VolunteerId && a.End == null);

            if (currentAssignment != null) // Le volontaire a un assignment actif
            {
                if (now - currentAssignment.Begin > TimeSpan.FromMinutes(40))
                {
                    // Plus de 30 minutes : marquer comme complété
                    var updatedAssignment = currentAssignment with
                    {
                        End = now,
                        MyEndStatus = DO.EndStatus.Completed
                    };
                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(updatedAssignment);
                    volunteersUpdate = true;
                    Observers.NotifyItemUpdated(v.VolunteerId);
                    Observers.NotifyItemUpdated(updatedAssignment.CallId);

                }
                else if (i % 4 == 0) // Moins de 30 minutes et i divisible par 2
                {
                    // Marquer comme auto-annulé
                    var updatedAssignment = currentAssignment with
                    {
                        End = now,
                        MyEndStatus = DO.EndStatus.SelfCancelled
                    };

                    calls.Add(s_dal.Call.Read(currentAssignment.CallId)!);

                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(updatedAssignment);
                    volunteersUpdate = true;
                    Observers.NotifyItemUpdated(v.VolunteerId);
                    Observers.NotifyItemUpdated(updatedAssignment.CallId);
                }
            }
            else if(i % 2 != 0) // Le volontaire n'a pas d'assignment actif
            {
                // Filtrer les calls dans la distance maximale du volontaire
                var availableCalls = calls.Where(call => CallManager.CalculOfDistance(call, v) < v.MaxDistance).ToList();
                //var availableCalls = calls.ToList();

                if (availableCalls.Any())
                {
                    // Sélectionner un call au hasard
                    var randomCall = availableCalls[random.Next(availableCalls.Count)];

                    // Créer un nouvel assignment
                    DO.Assignment newAssignment = new DO.Assignment
                    {
                        CallId = randomCall.CallId,
                        VolunteerId = v.VolunteerId,
                        Begin = now,
                        End = null,
                        MyEndStatus = null
                    };

                    calls.Remove(randomCall);

                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Create(newAssignment);

                    volunteersUpdate = true;
                    Observers.NotifyItemUpdated(v.VolunteerId);
                    Observers.NotifyItemUpdated(randomCall.CallId);

                }
            }

            i++;
        }

        if (volunteersUpdate)
            Observers.NotifyListUpdated();
    }
}

