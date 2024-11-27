using DalApi;
using DO;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        List<Course> Courses = XMLTools.LoadListFromXMLSerializer<Course>(Config.s_courses_xml);
        if (Courses.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Courses, Config.s_courses_xml);

    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Assignment? Read(int id)
    {
        throw new NotImplementedException();
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);

    }
}
