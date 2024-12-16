﻿namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

//use of XElement class
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Converts an XElement to a Volunteer object.
    /// </summary>
    /// <param name="v">The XElement representing a volunteer.</param>
    /// <returns>A Volunteer object.</returns>
    private static Volunteer getVolunteer(XElement v)
    {
        return new Volunteer
        (
            VolunteerId: (int?)v.Element("Id") ?? throw new FormatException("can't convert id"),
            Name: (string?)v.Element("Name") ?? "",
            PhoneNumber: (string?)v.Element("PhoneNumber") ?? "",
            Email: (string?)v.Element("Email") ?? "",
            Password: (string?)v.Element("Password"),
            Address: (string?)v.Element("Address"),
            Latitude: (double?)v.Element("Latitude"),
            Longitude: (double?)v.Element("Longitude"),
            MyJob: (Job)Enum.Parse(typeof(Job), (string?)v.Element("MyJob") ?? "Volunteer"),
            IsActive: (bool?)v.Element("active") ?? false,
            MaxDistance: (double?)v.Element("distance"),
            MyDistanceType: (DistanceType)Enum.Parse(typeof(DistanceType), (string?)v.Element("MyWhichDistance") ?? "AirDistance")
        );
    }

    /// <summary>
    /// Converts a Volunteer object to an XElement.
    /// </summary>
    /// <param name="v">The Volunteer object.</param>
    /// <returns>An XElement representing the volunteer.</returns>
    private static XElement createVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.VolunteerId),
            new XElement("Name", v.Name),
            new XElement("PhoneNumber", v.PhoneNumber),
            new XElement("Email", v.Email),
            new XElement("Password", v.Password),
            new XElement("Address", v.Address),
            new XElement("Latitude", v.Latitude),
            new XElement("Longitude", v.Longitude),
            new XElement("MyJob", v.MyJob),
            new XElement("active", v.IsActive),
            new XElement("distance", v.MaxDistance),
            new XElement("MyWhichDistance", v.MyDistanceType)
        );
    }

    /// <summary>
    /// Creates a new volunteer.
    /// </summary>
    /// <param name="item">The volunteer to create.</param>
    /// <exception cref="DalAlreadyExistException">Thrown when a volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteersRootElem.Elements().Any(v => (int?)v.Element("Id") == item.VolunteerId))
            throw new DalAlreadyExistException($"Volunteer with ID={item.VolunteerId} already exists");

        volunteersRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes a volunteer by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == id);
        if (volunteerElem == null)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes all volunteers.
    /// </summary>
    public void DeleteAll()
    {
        XElement volunteersRootElem = new XElement("Volunteers");
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Reads a volunteer by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to read.</param>
    /// <returns>The volunteer with the specified ID, or throw exception if it's null.</returns>
    public Volunteer Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(v => (int?)v.Element("Id") == id);
        return volunteerElem is null ? throw new DalDoesNotExistException($"Volunteer with the ID={id} does not exists") : getVolunteer(volunteerElem);
    }

    /// <summary>
    /// Reads a volunteer by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <returns>The first volunteer that matches the filter,or throw exception if it's null.</returns>
    public Volunteer Read(Func<Volunteer, bool> filter)
    {
        var volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
        return volunteerElem is null ? throw new DalDoesNotExistException($"Volunteer with this criteria does not exists") : volunteerElem;

    }

    /// <summary>
    /// Reads all volunteers, optionally filtered by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply, or null to read all volunteers.</param>
    /// <returns>An enumerable of volunteers that match the filter, or throw exception if it's null and throw exception if it's empty.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v));
        var volunteersFiltered = filter == null ? volunteers : volunteers.Where(filter);
        return volunteersFiltered == null ? throw new DalDoesNotExistException($"Volunteers with this criteria don't exist") : volunteersFiltered;
    }

    /// <summary>
    /// Updates an existing volunteer.
    /// </summary>
    /// <param name="item">The volunteer to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteerElem = volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.VolunteerId);
        if (volunteerElem == null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.VolunteerId} does not exist");

        volunteerElem.Remove();
        volunteersRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
}
