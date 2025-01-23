namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
/// The CallImplementations file contains implementations of methods and
/// functionalities related to call operations within the application. It defines 
/// how calls are managed, including their creation, updating, and other related actions.
/// </summary>
internal class CallImplementations : ICall
{
    /// <summary>
    /// Creates a new call and adds it to the data source.
    /// </summary>
    /// <param name="call">The call to be added.</param>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Call call)
    {
        int newCallId = Config._nextCallId;
        Call newCall = call with { CallId = newCallId };
        DataSource.Calls.Add(newCall);
    }

    /// <summary>
    /// Deletes a call with the specified ID from the data source.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        foreach (var call in DataSource.Calls)
        {
            if (call.CallId == id)
            {
                DataSource.Calls.Remove(call);
                return;
            }
        }
        throw new DalDoesNotExistException($"Call with the ID={id} does not exist in the system!");
    }

    /// <summary>
    /// Deletes all calls from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Reads and returns a call with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the call to be read.</param>
    /// <returns>The call with the specified ID, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(int id) => DataSource.Calls.FirstOrDefault(call => call.CallId == id);

    /// <summary>
    /// Reads and returns the first call that matches the specified filter condition.
    /// </summary>
    /// <param name="filter">The filter condition to match.</param>
    /// <returns>The first call that matches the filter condition, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(Func<Call, bool> filter) => DataSource.Calls.FirstOrDefault(filter);

    /// <summary>
    /// Reads and returns all calls, optionally filtered by the specified condition.
    /// </summary>
    /// <param name="filter">The filter condition to match, or null to return all calls.</param>
    /// <returns>A collection of calls that match the filter condition, or all calls if no filter is specified.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
        => filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

    /// <summary>
    /// Updates an existing call in the data source.
    /// </summary>
    /// <param name="item">The call with updated information.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Call item)
    {
        foreach (var call in DataSource.Calls)
        {
            if (call.CallId == item.CallId)
            {
                DataSource.Calls.Remove(call);
                DataSource.Calls.Add(item);
                return;
            }
        }
        throw new DalDoesNotExistException($"Call with the ID={item.CallId} does not exist in the system!");
    }
}
