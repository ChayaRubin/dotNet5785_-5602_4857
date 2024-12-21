

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
namespace Dal;
/// <summary>
/// A new AssignmentImplementation class that inherits from IAssignment.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("Can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("Can't convert CallId"),
            EntryTime = a.ToDateTimeNullable("EntryTime") ?? throw new FormatException("Can't convert EntryTime"),
            FinishCompletionTime = a.ToDateTimeNullable("FinishCompletionTime") ?? throw new FormatException("Can't convert EntryTime"),
            callResolutionStatus = a.ToEnumNullable<CallResolutionStatus>("callResolutionStatus") ?? throw new FormatException("Can't convert callResolutionStatus")
        };

    }

    /// <summary>
    /// Adds a new Call to the XML file.
    /// </summary>
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    public Assignment? Read(int id)
    {
        XElement? studentElem =
    XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return studentElem is null ? null : getAssignment(studentElem);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(s => getAssignment(s)).FirstOrDefault(filter);
    }

    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                .Remove();

        assignmentsRootElem.Add(new XElement("Assignment", createAssignmentElement(item)));

        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }


}
