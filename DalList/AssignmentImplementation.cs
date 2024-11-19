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
        Assignment newAssignment = assignment with { Id = newId, CallId=newCallId };

        // Add the reference of the copy to the list of assignments
        DataSource.Assignments.Add(newAssignment);

    }

    public void Delete(int id)
    {
        //Going through the list of assignments checking whether the id is the same
        foreach (var assignment in DataSource.Assignments)
        {
            //If so deleting the assignment
            if (assignment.Id == id)
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
    public Assignment? Read(int id)=> DataSource.Assignments.FirstOrDefault(assignment => assignment.Id == id);

    //returning the first Assignment object from DataSource.Assignments that satisfies the
    //filter condition, or null if no such object is found
    public Assignment? Read(Func<Assignment, bool> filter) => DataSource.Assignments.FirstOrDefault(filter);

    //returning an IEnumerable<Assignment>, which is a collection of Assignment objects.
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);

    public void Update(Assignment item)
    {
        //Going through the list of assignments checking whether the id is the same 
        foreach (var assignment in DataSource.Assignments)
        {
            if (assignment.Id == item.Id)
            {
                //If so deleting the assignment and adding the updated item
                DataSource.Assignments.Remove(assignment);
                DataSource.Assignments.Add(item);
                return;
            }
        }
        //Throwing exception if the id doesn't exist
        throw new DalDoesNotExistException($"Assignment with the ID={item.Id} does not exist in the system!");
    }

}

