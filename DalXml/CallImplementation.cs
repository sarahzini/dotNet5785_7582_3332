﻿namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//use of XmlSerializer class
internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new Call record and assigns it a unique ID.
    /// </summary>
    /// <param name="item">The Call object to be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        item = item with { CallId = Config.NextCallId };
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Reads a Call record by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to be read.</param>
    /// <returns>The Call object if found, otherwise null.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(it => it.CallId == id);
    }

    /// <summary>
    /// Reads a Call record that matches a given filter.
    /// </summary>
    /// <param name="filter">A function to filter the Call records.</param>
    /// <returns>The first Call object that matches the filter, otherwise null.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Updates an existing Call record.
    /// </summary>
    /// <param name="item">The Call object with updated information.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.CallId == item.CallId) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.CallId} does not exist");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a Call record by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.CallId == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all Call records.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Reads all Call records, optionally filtered by a given condition.
    /// </summary>
    /// <param name="filter">A function to filter the Call records (optional).</param>
    /// <returns>An IEnumerable of Call objects that match the filter, or all Call objects if no filter is provided.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ? calls : calls.Where(filter);
    }
}
