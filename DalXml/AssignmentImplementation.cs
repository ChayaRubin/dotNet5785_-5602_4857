
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    static Assignments getAssignment(XElement a)
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

        public Assignment? Read(int id)
        {
            XElement? studentElem =
        XMLTools.LoadListFromXMLElement(Config.s_assignment_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
            return studentElem is null ? null : getAssignment(studentElem);
        }

        public Assignment? Read(Func<Assignment, bool> filter)
        {
            return XMLTools.LoadListFromXMLElement(Config.s_assignment_xml).Elements().Select(s => getAssignment(s)).FirstOrDefault(filter);
        }

        public void Update(Assignment item)
        {
            XElement studentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignment_xml);

            (studentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
            ?? throw new DO.DalDoesNotExistException($"Student with ID={item.Id} does Not exist"))
                    .Remove();

            studentsRootElem.Add(new XElement("Student", createAssignmentElement(item)));

            XMLTools.SaveListToXMLElement(studentsRootElem, Config.s_assignment_xml);
        }
    

}
