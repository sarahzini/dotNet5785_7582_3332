namespace Dal;
using DalApi;
using DO;

/// <summary>
/// The AssignmentImplementations file contains implementations of methods and
/// functionalities related to assignment operations within the application. It defines 
/// how assignments are managed, including their creation, updating, and other related actions.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment and adds it to the data source.
    /// </summary>
    /// <param name="assignment">The assignment to be added.</param>
    public void Create(Assignment assignment)
    {
        int newId = Config._nextAssignmentId;
        Assignment newAssignment = assignment with { AssignmentId = newId };
        DataSource.Assignments.Add(newAssignment);
    }

    /// <summary>
    /// Deletes an assignment with the specified ID from the data source.
    /// </summary>
    /// <param name="id">The ID of the assignment to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        foreach (var assignment in DataSource.Assignments)
        {
            if (assignment.AssignmentId == id)
            {
                DataSource.Assignments.Remove(assignment);
                return;
            }
        }
        throw new DalDoesNotExistException($"Assignment with the ID={id} does not exist in the system!");
    }

    /// <summary>
    /// Deletes all assignments from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Reads and returns an assignment with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to be read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    public Assignment? Read(int id) => DataSource.Assignments.FirstOrDefault(assignment => assignment.AssignmentId == id);

    /// <summary>
    /// Reads and returns the first assignment that matches the specified filter condition.
    /// </summary>
    /// <param name="filter">The filter condition to match.</param>
    /// <returns>The first assignment that matches the filter condition, or null if not found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter) => DataSource.Assignments.FirstOrDefault(filter);

    /// <summary>
    /// Reads and returns all assignments, optionally filtered by the specified condition.
    /// </summary>
    /// <param name="filter">The filter condition to match, or null to return all assignments.</param>
    /// <returns>A collection of assignments that match the filter condition, or all assignments if no filter is specified.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);

    /// <summary>
    /// Updates an existing assignment in the data source.
    /// </summary>
    /// <param name="item">The assignment with updated information.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        foreach (var assignment in DataSource.Assignments)
        {
            if (assignment.AssignmentId == item.AssignmentId)
            {
                DataSource.Assignments.Remove(assignment);
                DataSource.Assignments.Add(item);
                return;
            }
        }
        throw new DalDoesNotExistException($"Assignment with the ID={item.AssignmentId} does not exist in the system!");
    }
}
