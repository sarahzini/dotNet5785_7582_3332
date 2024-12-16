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
    public void Create(Volunteer item)
    {
        //Going trough the list of volunteers and checking wether the
        //id of the volunteer is the same as the id of the item
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.VolunteerId == item.VolunteerId)
                //help by AI we didn't know how to trow exception
                throw new DalAlreadyExistException($"Volunteer with the ID={item.VolunteerId} already exists");
        }
        //Adding the item to the list after checking that the id is unique
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        //Going through the list of volunteers  checking wether  the id is the same
        foreach (var volunteer in DataSource.Volunteers)
        {
            //If so deleting the volunteer 
            if (volunteer.VolunteerId == id)
            {

                DataSource.Volunteers.Remove(volunteer);
                return;
            }
        }
        //Trowing exception if the id doesn't exist
        throw new DalDoesNotExistException($"Volunteer with the ID={id} does not exists");

    }
    public void DeleteAll()
    {
        // Clear all volunteers from the list
        DataSource.Volunteers.Clear();
    }

    //returning the volunteer if the id is the same
    public Volunteer Read(int id)
    {
        Volunteer? volunteer= DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == id);
        return volunteer is null ? throw new DalDoesNotExistException($"Volunteer with this Id{id} does not exist in the system! ") : volunteer;
    } 

    //returning the first Volunteer object from DataSource.Volunteers that satisfies the
    //filter condition, or null if no such object is found
    public Volunteer Read(Func<Volunteer, bool> filter)
    {
        Volunteer? volunteers= DataSource.Volunteers.FirstOrDefault(filter);
        return volunteers is null ? throw new DalDoesNotExistException($"Volunteers with this criteria don't exist in the system! ") : volunteers;
    }


    //returning an IEnumerable<Volunteer>, which is a collection of Volunteer objects.
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        IEnumerable<Volunteer> volunteers= filter == null ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);
        return volunteers is null ? throw new DalDoesNotExistException($"Volunteers with this criteria don't exist in the system! ") : volunteers;
    }
    public void Update(Volunteer item)
    {
        //Going through the list of volunteers  checking wether  the id is the same 
        foreach (var volunteer in DataSource.Volunteers)
        {

            if (volunteer.VolunteerId == item.VolunteerId)
            {
                //If so deleting the volunteer and adding the uptaded item
                DataSource.Volunteers.Remove(volunteer);
                DataSource.Volunteers.Add(item);
                return;
            }
        }
        //Trowing exception if the id doesn't exist
        throw new DalDoesNotExistException($"Volunteer with the ID={item.VolunteerId} does not exists");

    }
}
