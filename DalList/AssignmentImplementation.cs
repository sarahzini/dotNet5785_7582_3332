namespace Dal;
using DalApi;
using DO;
using DalList;
public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment assignment)
    {
        //This method was written by the AI and we explain it 

        // Generate a new ID using the next running number from DataSource.Config
        int newId = Config._nextAssignmentId;

        // Create a copy of the assignment object and update its ID with the new running number
        Assignment newAssignment = assignment with { Id = newId };

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
        throw new InvalidOperationException("An assignment witth this ID does not exist in the system!");
    }

    public void DeleteAll()
    {
        // Clear all assignments from the list
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        //Checking for each assignment if they have the same Ids, if they do return the assignment
        foreach (var assignment in DataSource.Assignments)
        {
            if (assignment.Id == id)
                return assignment;
        }
        //Else return null
        return null;
    }

    //Returning the copy of the list of assignments (New List)
    public List<Assignment> ReadAll() => new List<DO.Assignment>(DataSource.Assignments);

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
        throw new InvalidOperationException("An assignment with this ID does not exist in the system!");
    }

}

