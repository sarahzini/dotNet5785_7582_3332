﻿namespace Dal;
using DalApi;
using DO;

/// <summary>
/// The CallImplementations file contains implementations of methods and
/// functionalities related to call operations within the application. It defines 
/// how calls are managed, including their creation, updating, and other related actions.
/// </summary>

public class CallImplementations : ICall
{
    public void Create(Call call)
    {
        // Generate a new CallId using the next running number from Config
        int newCallId = Config._nextCallId;

        // Create a copy of the call object and update its ID with the new running number
        Call newCall = call with { Id = newCallId };

        // Add the reference of the copy to the list of calls
        DataSource.Calls.Add(newCall);
    }

    public void Delete(int id)
    {
        // Going through the list of calls checking whether the id is the same
        foreach (var call in DataSource.Calls)
        {
            // If so, deleting the call
            if (call.Id == id)
            {
                DataSource.Calls.Remove(call);
                return;
            }
        }
        // Throwing an exception if the id doesn't exist
        throw new Exception($"Call with the ID={id} does not exist in the system!");
    }

    public void DeleteAll()
    {
        // Clear all calls from the list
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        // Checking for each call if they have the same Ids, if they do return the call
        foreach (var call in DataSource.Calls)
        {
            if (call.Id == id)
                return call;
        }
        // Else return null
        return null;
    }

    // Returning the copy of the list of calls (New List)
    public List<Call> ReadAll() => new List<DO.Call>(DataSource.Calls);

    public void Update(Call item)
    {
        // Going through the list of calls checking whether the id is the same
        foreach (var call in DataSource.Calls)
        {
            if (call.Id == item.Id)
            {
                // If so, deleting the call and adding the updated item
                DataSource.Calls.Remove(call);
                DataSource.Calls.Add(item);
                return;
            }
        }
        // Throwing an exception if the id doesn't exist
        throw new Exception($"Call with the ID={item.Id} does not exist in the system!");
    }
}