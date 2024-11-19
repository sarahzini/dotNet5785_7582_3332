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
internal class VolunteerImplementations : IVolunteer
{
    public void Create(Volunteer item)
    {
        //Going trough the list of volunteers and checking wether the
        //id of the volunteer is the same as the id of the item
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.Id == item.Id)
                //help by AI we didn't know how to trow exception
                throw new Exception($"Volunteer with the ID={item.Id} already exists");
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
            if (volunteer.Id == id)
            {

                DataSource.Volunteers.Remove(volunteer);
                return;
            }
        }
        //Trowing exception if the id doesn't exist
        throw new Exception($"Volunteer with the ID={id} does not exists");

    }


    public void DeleteAll()
    {
        // Clear all volunteers from the list
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        //checking for each volunteer if they have have the same Ids if they do return the volunteer
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.Id == id)
                return volunteer;
        }
        //else return null
        return null;
    }

    //Returning the copy of the list of volunteers (New List)
    public List<Volunteer> ReadAll() => new List<DO.Volunteer>(DataSource.Volunteers);



    public void Update(Volunteer item)
    {
        //Going through the list of volunteers  checking wether  the id is the same 
        foreach (var volunteer in DataSource.Volunteers)
        {

            if (volunteer.Id == item.Id)
            {
                //If so deleting the volunteer and adding the uptaded item
                DataSource.Volunteers.Remove(volunteer);
                DataSource.Volunteers.Add(item);
                return;
            }
        }
        //Trowing exception if the id doesn't exist
        throw new Exception($"Volunteer with the ID={item.Id} does not exists");

    }
}
