using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dal;
/// <summary>
/// A new AssignmentImplementation class that inherits from IAssignment.
/// </summary>
internal class AssignmentImplementation : IAssignment
{


    /// <summary>
    /// Converts an XML element into an Assignment object.
    /// <summary>
    static Assignment getAssignment(XElement a)
    {
        var status = a.ToEnumNullable<CallResolutionStatus>("callResolutionStatus");
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? 0,
            CallId = a.ToIntNullable("CallId") ?? 0,
            VolunteerId = a.ToIntNullable("VolunteerId") ?? 0,
            EntryTime = a.ToDateTimeNullable("EntryTime") ?? new DateTime(2025, 3, 27),
            FinishCompletionTime = a.ToDateTimeNullable("FinishCompletionTime"),
            CallResolutionStatus = status
        };
    }

    /// <summary>
    /// Creates a new XML element representing an Assignment object, with its properties serialized into corresponding child elements.
    /// </summary>
    XElement CreateXElement(Assignment item)
    {
        return new XElement("Assignment",
            new XElement("Id", item.Id),
            new XElement("CallId", item.CallId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("EntryTime", item.EntryTime),
            new XElement("FinishCompletionTime", item.FinishCompletionTime),
            new XElement("callResolutionStatus", item.CallResolutionStatus));
    }


    /// <summary>
    /// Adds a new Assignment to the XML file.
    /// </summary>
    public void Create(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentsRootElem.Add(CreateXElement(item));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes a Assignment by ID from the XML file.
    /// </summary>
    public void Delete(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        XElement? toDelete = assignmentsRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
        if (toDelete != null)
        {
            toDelete.Remove();
            XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
        }
        else
        {
            throw new DO.DalDoesNotExistException($"Assignment with ID={id} does not exist");
        }
    }


    /// <summary>
    /// Deletes all Assignments from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XElement assignmentsRootElem = new XElement("Assignments");
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements()
                                        .FirstOrDefault(a => (int?)a.Element("Id") == id);
        return assignmentElem is null ? null : getAssignment(assignmentElem);
    }


    /// <summary>
    /// Reads the first Assignment matching a filter.
    /// </summary>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
            .Elements("Assignment")
            .Select(a => getAssignment(a))
            .ToList();

        Console.WriteLine($"Loaded {assignments.Count} assignments from XML.");

        var assignment = assignments.FirstOrDefault(a =>
        {
            bool result = filter(a);
            Console.WriteLine($"Checking Assignment {a.Id}: {result}");
            return result;
        });

        if (assignment != null)
        {
            Console.WriteLine($"Found matching assignment: {assignment.Id}");
        }
        else
        {
            Console.WriteLine("No matching assignment found.");
        }

        return assignment;
    }



    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
            .Elements("Assignment")
            .Select(a => getAssignment(a))
            .ToList(); // טוען את כל הרשימה

        Console.WriteLine($"Loaded {assignments.Count} assignments from XML.");

        if (filter != null)
        {
            assignments = assignments.Where(a =>
            {
                bool result = filter(a);
                Console.WriteLine($"Filtering Assignment {a.Id}: {result}");
                return result;
            }).ToList();
        }

        Console.WriteLine($"Returning {assignments.Count} assignments after filtering.");
        return assignments;
    }



    /// <summary>
    /// Updates an existing Assignment in the XML file.
    /// </summary>
    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                .Remove();

        /*        assignmentsRootElem.Add(new XElement("Assignment", CreateXElement(item)));*/
        assignmentsRootElem.Add(CreateXElement(item));


        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }
}
