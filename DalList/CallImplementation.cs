namespace Dal;
using DalApi;
using DO;

/// <summary>
/// The CallImplementations file contains implementations of methods and
/// functionalities related to call operations within the application. It defines 
/// how calls are managed, including their creation, updating, and other related actions.
/// </summary>
internal class CallImplementations : ICall
{
    public void Create(Call call)
    {
        // Generate a new CallId using the next running number from Config
        int newCallId = Config._nextCallId;

        // Create a copy of the call object and update its ID with the new running number
        Call newCall = call with { CallId = newCallId };

        // Add the reference of the copy to the list of calls
        DataSource.Calls.Add(newCall);
    }

    public void Delete(int id)
    {
        // Going through the list of calls checking whether the id is the same
        foreach (var call in DataSource.Calls)
        {
            // If so, deleting the call
            if (call.CallId == id)
            {
                DataSource.Calls.Remove(call);
                return;
            }
        }
        // Throwing an exception if the id doesn't exist
        throw new DalDoesNotExistException($"Call with the ID={id} does not exist in the system! ");
    }

    public void DeleteAll()
    {
        // Clear all calls from the list
        DataSource.Calls.Clear();
    }
    public Call Read(int id)
    {
        Call? call = DataSource.Calls.FirstOrDefault(call => call.CallId == id);
        return call is null ? throw new DalDoesNotExistException($"Call with the ID={id} does not exist in the system! ") : call;
    }

    //returning the first Call object from DataSource.Calls that satisfies the
    //filter condition, or null if no such object is found
    public Call Read(Func<Call, bool> filter)
    {
        Call? call =DataSource.Calls.FirstOrDefault(filter);
        return call is null ? throw new DalDoesNotExistException($"Call with this criteria does not exist in the system! ") : call;

    }

    //returning an IEnumerable<Call>, which is a collection of Call objects.
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        IEnumerable<Call>? calls = filter == null ? DataSource.Calls.Select(item => item)
             : DataSource.Calls.Where(filter);
        return calls is null ? throw new DalDoesNotExistException($"Calls with this criteria don't exist in the system! ") : calls;
    }

    public void Update(Call item)
    {
        // Going through the list of calls checking whether the id is the same
        foreach (var call in DataSource.Calls)
        {
            if (call.CallId == item.CallId)
            {
                // If so, deleting the call and adding the updated item
                DataSource.Calls.Remove(call);
                DataSource.Calls.Add(item);
                return;
            }
        }
        // Throwing an exception if the id doesn't exist
        throw new DalDoesNotExistException($"Call with the ID={item.CallId} does not exist in the system!");
    }
}