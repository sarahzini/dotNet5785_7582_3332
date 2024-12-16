using DalApi;
using DO;

namespace Dal;

//use of XmlSerializer class

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="item">The assignment that will be created.</param>
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        item = item with { AssignmentId = Config.NextAssignmentId, CallId = Config.NextCallId };
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(it => it.AssignmentId == id) == 0)
            throw new DalDoesNotExistException($"Assignment with this ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all assignments.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or throw exception if it's null.</returns>
    public Assignment Read(int id)
    {
        Assignment? assignment = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml).FirstOrDefault(a => a.AssignmentId == id);
        return assignment is null ? throw new DalDoesNotExistException($"Assignment with the ID={id} does not exists")  : assignment;
    }

    /// <summary>
    /// Reads an assignment by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <returns>The first assignment that matches the filter, or throw exception if it's null.</returns>
    public Assignment Read(Func<Assignment, bool> filter)
    {
        Assignment? assignment = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml).FirstOrDefault(filter);
        return assignment is null ? throw new DalDoesNotExistException($"Assignment with this criteria does not exists") : assignment;
    }

    /// <summary>
    /// Reads all assignments, optionally filtered by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply, or null to read all assignments.</param>
    /// <returns>An enumerable of assignments that match the filter, or all assignments if no filter is specified and throw exception if it's empty.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        var assignmentsFiltered = filter == null ? assignments : assignments.Where(filter);
        return assignments == null ? throw new DalDoesNotExistException($"Assignments with this criteria don't exist") : assignments;
    }

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(it => it.AssignmentId == item.AssignmentId) == 0)
            throw new DalDoesNotExistException($"Assignment with this ID={item.AssignmentId} does not exist");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }
}
