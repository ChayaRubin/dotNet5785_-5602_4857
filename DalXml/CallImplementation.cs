namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

/// <summary>
/// A new CallImplementation class that inherits from ICall.
/// </summary>
internal class CallImplementation :ICall
{
    /// <summary>
    /// Adds a new Call to the XML file.
    /// </summary>
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a Call by ID from the XML file.
    /// </summary>
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.RadioCallId == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist.");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all Calls from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Reads the first Call matching a filter.
    /// </summary>
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Calls, optionally filtered by a predicate.
    /// </summary>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ? calls : calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing Call in the XML file.
    /// </summary>
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.RadioCallId == item.RadioCallId) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.RadioCallId} does not exist.");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }
}