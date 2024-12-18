
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
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
        throw new NotImplementedException();
    }
}
