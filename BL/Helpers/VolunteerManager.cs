﻿using BlApi;
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
        if(oldVolunteer?.MyJob != updatedVolunteer.MyJob&& !isManager)
        { throw new BLInvalidOperationException("You cannot change this member because you are not a Manager."); }
    }

    internal static void SimulateVolunteerSystem()
    {
        // Flag to indicate if any volunteer data was updated
        bool volunteersUpdate = false;

        // Random number generator for selecting calls
        Random random = new Random();

        IEnumerable<DO.Volunteer>? volunteers;
        List<DO.Call>? calls;
        IEnumerable<DO.Assignment>? assignments;

        // Locking the BLMutex to ensure thread safety while reading data
        lock (AdminManager.BlMutex)
        {
            // Fetch all active volunteers, calls, and assignments
            volunteers = s_dal.Volunteer.ReadAll()!.Where(v => v.IsActive == true).ToList();
            calls = s_dal.Call.ReadAll()!.ToList();
            assignments = s_dal.Assignment.ReadAll()!.ToList();
        }

        DateTime now;
        // Locking the BLMutex again to get the current time in a thread-safe manner
        lock (AdminManager.BlMutex)
            now = AdminManager.Now;

        // Filtering the calls that meet certain conditions
        List<DO.Call> filteredCalls = new List<DO.Call>(calls!);

        foreach (var C in calls!)
        {
            DO.Assignment? a;
            // Locking to get the latest assignment for each call
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

            // Check if the call has passed its maximum end time or has no active assignment
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
            // Find the active assignment for the current volunteer
            var currentAssignment = assignments.FirstOrDefault(a => a.VolunteerId == v.VolunteerId && a.End == null);

            if (currentAssignment != null) // If the volunteer has an active assignment
            {
                if (now - currentAssignment.Begin > TimeSpan.FromMinutes(40))
                {
                    // If the assignment has been ongoing for more than 40 minutes, mark it as completed
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
                else if (i % 4 == 0) // If less than 40 minutes and i is divisible by 4
                {
                    // Mark the assignment as self-cancelled
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
            else if (i % 2 != 0) // If the volunteer has no active assignment
            {
                // Filter calls within the maximum distance from the volunteer
                var availableCalls = calls.Where(call => CallManager.CalculOfDistance(call, v) < v.MaxDistance).ToList();

                if (availableCalls.Any())
                {
                    // Select a random call and assign it to the volunteer
                    var randomCall = availableCalls[random.Next(availableCalls.Count)];

                    // Create a new assignment for the volunteer
                    DO.Assignment newAssignment = new DO.Assignment
                    {
                        CallId = randomCall.CallId,
                        VolunteerId = v.VolunteerId,
                        Begin = now,
                        End = null,
                        MyEndStatus = null
                    };

                    // Remove the assigned call from the available list
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

        // If any volunteers were updated, notify the observers to refresh the data
        if (volunteersUpdate)
            Observers.NotifyListUpdated();
    }

}

