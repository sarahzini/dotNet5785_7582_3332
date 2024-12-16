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
    public void Create(Assignment assignment)
    {
        //This method was written by the AI and we explain it 
        // Generate a new ID and CallId using the next running number from Config
        int newId = Config._nextAssignmentId;
        int newCallId = Config._nextCallId;

        // Create a copy of the assignment object and update its ID and CallId with the new running number
        Assignment newAssignment = assignment with { AssignmentId = newId, CallId=newCallId };

        // Add the reference of the copy to the list of assignments
        DataSource.Assignments.Add(newAssignment);

    }

    public void Delete(int id)
    {
        //Going through the list of assignments checking whether the id is the same
        foreach (var assignment in DataSource.Assignments)
        {
            //If so deleting the assignment
            if (assignment.AssignmentId == id)
            {
                DataSource.Assignments.Remove(assignment);
                return;
            }
        }
        //Throwing exception if the id doesn't exist
        throw new DalDoesNotExistException($"Assignment with the ID={id} does not exist in the system!");
    }

    public void DeleteAll()
    {
        // Clear all assignments from the list
        DataSource.Assignments.Clear();
    }

    //returning the assignment if the id is the same
    public Assignment Read(int id)
    {
        Assignment? assignment= DataSource.Assignments.FirstOrDefault(assignment => assignment.AssignmentId == id);
        return assignment is null ? throw new DalDoesNotExistException($"Assignment with this Id{id} does not exist in the system! ") : assignment;
    }

    //returning the first Assignment object from DataSource.Assignments that satisfies the
    //filter condition, or null if no such object is found
    public Assignment Read(Func<Assignment, bool> filter)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(filter);
        return assignment is null ? throw new DalDoesNotExistException($"Assignment with this criteria does not exist in the system! ") : assignment;

    }

    //returning an IEnumerable<Assignment>, which is a collection of Assignment objects.
    public IEnumerable<Assignment>? ReadAll(Func<Assignment, bool>? filter = null)
    {
        //    IEnumerable<Assignment> assignments= filter == null ? DataSource.Assignments.Select(item => item)
        //        : DataSource.Assignments.Where(filter);
        //    return assignments is null? throw new DalDoesNotExistException($"Assignments with this criteria does not exist in the system! ") : assignments;

        return filter == null ? DataSource.Assignments.Select(item => item)
           : DataSource.Assignments.Where(filter);
    }


    public void Update(Assignment item)
    {
        //Going through the list of assignments checking whether the id is the same 
        foreach (var assignment in DataSource.Assignments)
        {
            if (assignment.AssignmentId == item.AssignmentId)
            {
                //If so deleting the assignment and adding the updated item
                DataSource.Assignments.Remove(assignment);
                DataSource.Assignments.Add(item);
                return;
            }
        }
        //Throwing exception if the id doesn't exist
        throw new DalDoesNotExistException($"Assignment with the ID={item.AssignmentId} does not exist in the system!");
    }

}

