namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.ComponentModel.Design;

/// <summary>
/// The VolunteerImplementations file likely contains implementations of methods and
/// functionalities related to volunteer operations within the application. It defines 
/// how volunteers are managed, including their creation, updating, and other related actions.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new volunteer and adds it to the data source.
    /// </summary>
    /// <param name="item">The volunteer to be added.</param>
    /// <exception cref="DalAlreadyExistException">Thrown when a volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.VolunteerId == item.VolunteerId)
                throw new DalAlreadyExistException($"Volunteer with the ID={item.VolunteerId} already exists");
        }
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Deletes a volunteer with the specified ID from the data source.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.VolunteerId == id)
            {
                DataSource.Volunteers.Remove(volunteer);
                return;
            }
        }
        throw new DalDoesNotExistException($"Volunteer with the ID={id} does not exist");
    }

    /// <summary>
    /// Deletes all volunteers from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Reads and returns a volunteer with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be read.</param>
    /// <returns>The volunteer with the specified ID, or null if not found.</returns>
    public Volunteer? Read(int id) => DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == id);

    /// <summary>
    /// Reads and returns the first volunteer that matches the specified filter condition.
    /// </summary>
    /// <param name="filter">The filter condition to match.</param>
    /// <returns>The first volunteer that matches the filter condition, or null if not found.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter) => DataSource.Volunteers.FirstOrDefault(filter);

    /// <summary>
    /// Reads and returns all volunteers, optionally filtered by the specified condition.
    /// </summary>
    /// <param name="filter">The filter condition to match, or null to return all volunteers.</param>
    /// <returns>A collection of volunteers that match the filter condition, or all volunteers if no filter is specified.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
         => filter == null
             ? DataSource.Volunteers.Select(item => item)
             : DataSource.Volunteers.Where(filter);

    /// <summary>
    /// Updates an existing volunteer in the data source.
    /// </summary>
    /// <param name="item">The volunteer with updated information.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    public void Update(Volunteer item)
    {
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.VolunteerId == item.VolunteerId)
            {
                DataSource.Volunteers.Remove(volunteer);
                DataSource.Volunteers.Add(item);
                return;
            }
        }
        throw new DalDoesNotExistException($"Volunteer with the ID={item.VolunteerId} does not exist");
    }
}
