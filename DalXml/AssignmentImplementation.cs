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
    XElement CreateXElement(Assignment item)
    {
        return new XElement("Assignment",
            new XElement("Id", item.Id),
            new XElement("CallId", item.CallId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("EntryTime", item.EntryTime),
            new XElement("FinishCompletionTime", item.FinishCompletionTime),
            new XElement("callResolutionStatus", item.callResolutionStatus));
    }

    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("Can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
            EntryTime = a.ToDateTimeNullable("EntryTime") ?? throw new FormatException("Can't convert EntryTime"),
            FinishCompletionTime = a.ToDateTimeNullable("FinishCompletionTime") ?? throw new FormatException("Can't convert FinishCompletionTime"),
            callResolutionStatus = a.ToEnumNullable<CallResolutionStatus>("callResolutionStatus") ?? throw new FormatException("Can't convert callResolutionStatus")
        };
    }

    public void Create(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentsRootElem.Add(CreateXElement(item));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

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

    public void DeleteAll()
    {
        XElement assignmentsRootElem = new XElement("Assignments");
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

    //public Assignment? Read(int id)
    //{
    //    XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements()
    //                                    .FirstOrDefault(a => (int?)a.Element("Id") == id);
    //    return assignmentElem is null ? null : getAssignment(assignmentElem);
    //}

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a)).FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a));
        return filter is null ? assignments : assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                .Remove();

        assignmentsRootElem.Add(new XElement("Assignment", CreateXElement(item)));

        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }
}
