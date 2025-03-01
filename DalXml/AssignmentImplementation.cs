﻿using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal;

//use of XmlSerializer class

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="item">The assignment that will be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        item = item with { AssignmentId = Config.NextAssignmentId };
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
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
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(a => a.AssignmentId == id);
    }

    /// <summary>
    /// Reads an assignment by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <returns>The first assignment that matches the filter, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments, optionally filtered by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply, or null to read all assignments.</param>
    /// <returns>An enumerable of assignments that match the filter, or all assignments if no filter is specified.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(it => it.AssignmentId == item.AssignmentId) == 0)
            throw new DalDoesNotExistException($"Assignment with this ID={item.AssignmentId} does not exist");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }
}
